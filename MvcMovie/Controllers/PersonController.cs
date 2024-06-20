using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvcmovie.Data;
using mvcmovie.Models;
using mvcmovie.Models.Process;
using OfficeOpenXml;
using X.PagedList;
namespace mvcmovie.Controllers
{
    public class PersonController : Controller
    {
        private  readonly ApplicationDbContext _context;
        private ExcelProcess _excelProcess = new ExcelProcess();          
        public PersonController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        
        public async Task<IActionResult> Index(int? page, int? PageSize)
        {
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem() { Value="3", Text="3"},
                new SelectListItem() { Value="5", Text="5"},
                new SelectListItem() { Value="10",Text="10"},
                new SelectListItem() { Value="15",Text="15"},
                new SelectListItem() { Value="25",Text="25"},
                new SelectListItem() {Value="50",Text="50"},
            };
            int pagesize=(PageSize ?? 3);
            ViewBag.psize = pagesize;
            var model = _context.Person.ToList().ToPagedList(page ?? 1, pagesize);
        return View (model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string searchString)
        {
            if(_context.Person == null)
            {
                return Problem("Entity set 'mvcmovieContext.Person' is null.");
            }
            var person = from m in _context.Person 
            select m;
            if(!String.IsNullOrEmpty(searchString))
            {person = person.Where(s => s.FullName!.Contains(searchString));}
            return View(await person.ToListAsync());
        }
        
        public IActionResult Create()
        {
            return View ();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create (Person person)
        {
            if (ModelState.IsValid)
            {
                _context.Add(person);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(person);
        }
        
        public async Task<IActionResult> Edit(string id)
        {
            if (id==null || _context.Person == null)
            {
                return NotFound();
            }
            var person = await _context.Person.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }
            return View(person);
        }
        private bool PersonExists(string id)
    {
        return _context.Person.Any(p => p.PersonId == id);
    }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id ,Person person)
        {
          
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(person);
                    await _context.SaveChangesAsync();
                }
                catch(DbUpdateConcurrencyException)
                {
                    if( !PersonExists(person.PersonId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View (person);
        }
        public async Task<IActionResult> Delete(string id)
        {
            if (id==null || _context.Person == null)
            {
                return NotFound();
            }
            var person = await _context.Person
            .FirstOrDefaultAsync(m => m.PersonId == id);
            if (person == null)
            {
                return NotFound();
            }
            return View(person);
        }
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
             if ( _context.Person == null)
            {
                return Problem ("Entity set 'ApplicationDbContext.Person' is null.");
            }
            var person = await _context.Person.FindAsync(id);
            if (person != null)
            {
                _context.Person.Remove(person);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction (nameof(Index));
        }
        public async Task<IActionResult> Upload()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
           if(file==null){
            return BadRequest("Full null");
           }
           string fileExtension =Path.GetExtension(file.FileName);
           if(fileExtension.ToLower()!=".xlsx"&& fileExtension.ToLower()!=".xls"){
             return BadRequest("Full null");
           }
           var fileName = file.FileName;
           var filePath = Path.Combine(Directory.GetCurrentDirectory()+"/Uploads/Excels",fileName);
           var fileLocation = new FileInfo(filePath).ToString();
           using(var stream = new FileStream(filePath,FileMode.Create)){
            await file.CopyToAsync(stream);
            var dt =_excelProcess.ExcelToDataTable(fileLocation);
            for(int i=0;i<dt.Rows.Count;i++){
                  var ps = new Person();
                ps.PersonId=dt.Rows[i][0].ToString();
                ps.FullName=dt.Rows[i][1].ToString();
                ps.Address=dt.Rows[i][2].ToString();
                _context.Person.Add(ps);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
           }
           return View();
        }
        public async Task<IActionResult> DeleteAll(){
            var PersonList = _context.Person.ToList();  
            _context.Person.RemoveRange(PersonList);
            await _context.SaveChangesAsync();  
             return RedirectToAction(nameof(Index));
        }
        public IActionResult Download()
        {
            var fileName = "YourFileName" + ".xlsx";
            using(ExcelPackage excelPackage = new ExcelPackage())
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                worksheet.Cells["A1"].Value = "PersonId";
                worksheet.Cells["B1"].Value = "FullName";
                worksheet.Cells["C1"].Value = "Address";
                var personList = _context.Person.ToList();
                worksheet.Cells["A2"].LoadFromCollection(personList);
                var stream = new MemoryStream(excelPackage.GetAsByteArray());
                return File(stream,"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",fileName);
            }
    }
}
}