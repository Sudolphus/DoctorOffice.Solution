using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using DoctorOffice.Models;

namespace DoctorOffice.Controllers
{
  public class SpecialtiesController : Controller
  {
    private readonly DoctorOfficeContext _db;
    public SpecialtiesController(DoctorOfficeContext db)
    {
      _db = db;
    }

    public ActionResult Index(string specName)
    {
      IQueryable<Specialty> specialtyQuery = _db.Specialties;
      if (!string.IsNullOrEmpty(specName))
      {
        Regex search = new Regex(specName, RegexOptions.IgnoreCase);
        specialtyQuery = specialtyQuery.Where(specialties => search.IsMatch(specialties.Type));
      }
      IEnumerable<Specialty> model = specialtyQuery.ToList().OrderBy(specialty => specialty.Type);
      return View(model);
    }

    public ActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public ActionResult Create(Specialty specialty)
    {
      _db.Specialties.Add(specialty);
      _db.SaveChanges();
      return RedirectToAction("Details", new { id = specialty.SpecialtyId});
    }

    public ActionResult Details(int id)
    {
      var thisSpecialty = _db.Specialties
        .Include(specialty => specialty.Doctors) //review here
          .ThenInclude(join => join.Doctor)
        .FirstOrDefault(specialty => specialty.SpecialtyId == id);
      return View(thisSpecialty);  
    }

    public ActionResult Edit(int id)
    {
      var thisSpecialty = _db.Specialties.FirstOrDefault(specialty => specialty.SpecialtyId == id);
      ViewBag.DoctorId = new SelectList(_db.Doctors, "DoctorId", "Name");
      return View(thisSpecialty);
    }

    [HttpPost]
    public ActionResult Edit(Specialty specialty, int DoctorId)
    {
      if(DoctorId != 0)
      {
        _db.DoctorPatientSpecialty.Add(new DoctorPatientSpecialty() { DoctorId = DoctorId, SpecialtyId = specialty.SpecialtyId});
      }
      _db.Entry(specialty).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Details", new { id = specialty.SpecialtyId});
    }

    public ActionResult Delete(int id)
    {
      var thisSpecialty = _db.Specialties.FirstOrDefault(specialty => specialty.SpecialtyId == id);
      return View(thisSpecialty);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisSpecialty = _db.Specialties.FirstOrDefault(specialty => specialty.SpecialtyId == id);
      _db.Specialties.Remove(thisSpecialty);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}