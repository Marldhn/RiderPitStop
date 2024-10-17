
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiderPitStop.Models;
using RiderPitStop.Services;
using System.Collections.Generic;
using System.Linq;


namespace RiderPitStop.Controllers
{
    public class RequestOrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RequestOrderController(ApplicationDbContext context)
        {
            _context = context;
        }




        public IActionResult CompleteList()
        {
            var requestOrders = _context.RequestOrders
          .Include(ro => ro.Product)
          .Include(ro => ro.requestDetails)
          .Where(ro => ro.requestDetails.IsComplete)
          .ToList();

            return View(requestOrders);
        }

        public IActionResult Index()
        {
            var requestOrders = _context.RequestOrders
           .Include(ro => ro.Product)
           .Include(ro => ro.requestDetails)
           .Where(ro => !ro.requestDetails.IsComplete)
           .ToList();

            return View(requestOrders);
        }

        // Display the create request order form
        // Display the create request order form
        public IActionResult Create()
        {
            // Retrieve the logged-in user's username
            var username = User.Identity.Name;

            if (!string.IsNullOrEmpty(username))
            {
                // Assuming you have a Users table that includes FirstName and LastName
                var user = _context.Users.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    // Construct the "Prepared By" field using the user's first and last name
                    ViewBag.PreparedBy = $"{user.FirstName} {user.LastName}";
                }

            }


            ViewBag.Products = _context.Products.ToList(); // Load all products for selection

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DateTime dateTime, string note, int[] productIds, int[] quantities)
        {
            // Retrieve the logged-in user's username
            var username = User.Identity.Name;
            var preparedBy = "Unknown User";

            if (!string.IsNullOrEmpty(username))
            {
                var user = _context.Users.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    preparedBy = $"{user.FirstName} {user.LastName}";
                }
            }

            if (ModelState.IsValid)
            {
                // Create the RequestDetails entity
                var requestDetails = new RequestDetails
                {
                    PreparedBy = preparedBy, // Automatically populated based on logged-in user
                    DateTime = dateTime,
                    Note = note
                };

                // Save RequestDetails first to get DetailsId
                _context.RequestDetails.Add(requestDetails);
                _context.SaveChanges();

                // Consolidate quantities by ProductId
                var productQuantityMap = new Dictionary<int, int>();

                for (int i = 0; i < productIds.Length; i++)
                {
                    if (productQuantityMap.ContainsKey(productIds[i]))
                    {
                        productQuantityMap[productIds[i]] += quantities[i];
                    }
                    else
                    {
                        productQuantityMap[productIds[i]] = quantities[i];
                    }
                }

                // Add or update RequestOrder entries
                foreach (var entry in productQuantityMap)
                {
                    var productId = entry.Key;
                    var quantity = entry.Value;

                    var existingRequestOrder = _context.RequestOrders
                        .AsNoTracking()
                        .FirstOrDefault(ro => ro.ProductId == productId && ro.DetailsId == requestDetails.DetailsId);

                    if (existingRequestOrder == null)
                    {
                        var requestOrder = new RequestOrder
                        {
                            ProductId = productId,
                            DetailsId = requestDetails.DetailsId,
                            Quantity = quantity
                        };
                        _context.RequestOrders.Add(requestOrder);
                    }
                    else
                    {
                        existingRequestOrder.Quantity += quantity;
                        _context.RequestOrders.Update(existingRequestOrder);
                    }
                }

                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Products = _context.Products.ToList();
            return View();
        }




        public IActionResult CompleteDetails(int id)
        {
            var requestDetails = _context.RequestDetails
                .Include(rd => rd.RequestOrders) // Load associated RequestOrders
                    .ThenInclude(ro => ro.Product) // Load associated Products
                .FirstOrDefault(rd => rd.DetailsId == id);

            if (requestDetails == null)
            {
                return NotFound();
            }

            return View(requestDetails);
        }

        public IActionResult Details(int id)
        {
            var requestDetails = _context.RequestDetails
                .Include(rd => rd.RequestOrders) // Load associated RequestOrders
                    .ThenInclude(ro => ro.Product) // Load associated Products
                .FirstOrDefault(rd => rd.DetailsId == id);

            if (requestDetails == null)
            {
                return NotFound();
            }

            return View(requestDetails);
        }



        [HttpPost]
        public IActionResult MarkAsComplete(int detailsId)
        {
            var requestDetails = _context.RequestDetails
                .Include(rd => rd.RequestOrders)
                .ThenInclude(ro => ro.Product)
                .FirstOrDefault(rd => rd.DetailsId == detailsId);

            if (requestDetails != null)
            {
                try
                {
                    // Mark the request as complete
                    requestDetails.IsComplete = true;

                    // Update the product stock quantities based on the RequestOrders
                    foreach (var requestOrder in requestDetails.RequestOrders)
                    {
                        var product = requestOrder.Product;

                        if (product != null)
                        {
                            // Increment StockQuantity by the request order quantity
                            product.StockQuantity = (product.StockQuantity ?? 0) + requestOrder.Quantity;
                        }
                    }

                    // Save changes to the database
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating stock quantities: {ex.Message}");
                    // Optionally, handle the error here, such as by returning an error view or logging the error.
                }
            }

            return RedirectToAction(nameof(Index));
        }




    }
}
