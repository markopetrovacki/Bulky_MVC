using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
			
            return View(objProductList);
        }

        public IActionResult Upsert(int? id) //UpdateInsert
		{
     //       IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category
     //           .GetAll().Select(U => new SelectListItem
				 //{
					// Text = U.Name,
					// Value = U.Id.ToString(),
				 //});

			//ViewBag.CategoryList = CategoryList;
			//ViewData["CategoryList"] = CategoryList;

			ProductViewModel productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(U => new SelectListItem
				{
					Text = U.Name,
					Value = U.Id.ToString(),
				}),
                Product = new Product()
            };
            if(id== null || id == 0)
            {
                //Create
				return View(productVM);
			}
            else
            {
                //Update
                productVM.Product = _unitOfWork.Product.Get(u=>u.Id==id);
				return View(productVM);
			}

			
        }

        [HttpPost]
        public IActionResult Upsert(ProductViewModel productVM, IFormFile? file)
        {
            
            if(ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        //delete the old image
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                        if(System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using ( var fileStream = new FileStream(Path.Combine(productPath, fileName),FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }

                if(productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }

				//_unitOfWork.Product.Add(productVM.Product);
				_unitOfWork.Save();
                TempData["success"] = "Product create successfully";
                return RedirectToAction("Index");
            }
            else
            {

				productVM.CategoryList = _unitOfWork.Category.GetAll().Select(U => new SelectListItem
                {
                    Text = U.Name,
                    Value = U.Id.ToString(),
                });
				return View(productVM);
			}
            

        }

        //public IActionResult Edit(int id)
        //{
        //    if(id == null || id == 0)
        //    {
        //        return NotFound();
        //    }

        //    Product productFromDb = _unitOfWork.Product.Get(u => u.Id == id);
            
        //    if(productFromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(productFromDb);
        //}

        //[HttpPost]
        //public IActionResult Edit(Product obj)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Product.Update(obj);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Product update successfully";
        //        return RedirectToAction("Index");
        //    }
        //    return View();
        //}

        //public IActionResult Delete(int id)
        //{
        //    if(id == null | id == 0)
        //    {
        //        return NotFound();
        //    }

        //    Product productFromDb = _unitOfWork.Product.Get(u =>u.Id == id);

        //    if(productFromDb == null)
        //    {
        //        return NotFound() ;
        //    }

        //    return View(productFromDb);
        //}

        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePOST(int id)
        //{
        //    Product obj = _unitOfWork.Product.Get(u => u.Id == id);

        //    if(obj == null)
        //    {
        //        return NotFound();
        //    }

        //    _unitOfWork.Product.Remove(obj);
        //    _unitOfWork.Save();
        //    TempData["success"] = "Product deleted successfully";
        //    return RedirectToAction("Index");

        //}


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new {data = objProductList});
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u=>u.Id == id);
            if(productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }


        #endregion
    }
}
