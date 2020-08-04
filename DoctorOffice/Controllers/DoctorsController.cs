using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DoctorOffice.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DoctorOffice.Controllers
{
  public class DoctorsController : Controller
  {
    private readonly DoctorOfficeContext _db;
    public DoctorsController(DoctorOfficeContext db)
    {
      _db = db;
    }

    public ActionResult Index()
    {
      List<Doctor> model = _db.Doctors.ToList();
      return View(model);
    }

    public ActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public ActionResult Create(Doctor doctor)
    {
      _db.Doctors.Add(doctor);
      _db.SaveChanges();
      return RedirectToAction("Details", new { id = doctor.DoctorId });
    }

    public ActionResult Details(int id)
    {
      var thisDoctor = _db.Doctors
        .Include(doctor => doctor.PatientsSpecialties)
          .ThenInclude(join => join.Patient)
          .AlsoInclude(join => join.Specialty)
        .FirstOrDefault(doctor => doctor.DoctorId == id);
      return View(thisDoctor);
    }

    public ActionResult Edit()
    {
      var thisDoctor = _db.Doctors.FirstOrDefault(doctor => doctor.DoctorId == id);
      return View(thisDoctor);
    }

    [HttpPost]
    public ActionResult Edit(Doctor doctor, int PatientId, int SpecialtyId)
    {
      if(PatientId != 0)
      {
        _db.DoctorPatientSpecialty.Add(new DoctorPatientSpecialty() { PatientId = PatientId, DoctorId = doctor.DoctorId });
      }
      if(SpecialtyId != 0)
      {
        _db.DoctorPatientSpecialty.Add(new DoctorPatientSpecialty() { SpecialtyId = SpecialtyId, DoctorId = doctor.DoctorId });
      }
      _db.Entry(doctor).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Details", new { id = doctor.DoctorId });
    }

    public ActionResult Delete(int id)
    {
      var thisDoctor = _db.Doctors.FirstOrDefault(doctor => doctor.DoctorId == id);
      return View(thisDoctor);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisDoctor = _db.Doctors.FirstOrDefault(doctor => doctor.DoctorId == id);
      _db.Doctors.Remove(thisDoctor);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}