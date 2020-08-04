using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DoctorOffice.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

    public ActionResult Index(string docName) //passing in docName for query building filtering doctors by search
    {
      IQueryable<Doctor> doctorQuery = _db.Doctors;
      if (!string.IsNullOrEmpty(docName)) //if no string has been typed to filter by, then the alphabetical list is autopopulated
      {
        Regex search = new Regex(docName, RegexOptions.IgnoreCase); //replaces auto-populated alphabetical list with string search results
        doctorQuery = doctorQuery.Where(doctors => search.IsMatch(doctors.Name)); //matches user search string against doctor names if they contain string
      }
      IEnumerable<Doctor> model = doctorQuery.ToList().OrderBy(doctor => doctor.Name); //adjusting from list to IEnumerable allows us to use OrderBy method and sort alphabetically
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
        .Include(doctor => doctor.PatientsSpecialties)
          .ThenInclude(join => join.Specialty)
        .FirstOrDefault(doctor => doctor.DoctorId == id);
      return View(thisDoctor);
    }

    public ActionResult Edit(int id)
    {
      var thisDoctor = _db.Doctors.FirstOrDefault(doctor => doctor.DoctorId == id);
      ViewBag.PatientId = new SelectList(_db.Patients, "PatientId", "Name"); //allows us to pass in patient id to iterate but to display name rather than id in select list line 16-20 of views/doctor/edit
      ViewBag.SpecialtyId = new SelectList(_db.Specialties, "SpecialtyId", "Type"); //same as above but for specialties of the doctor
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

    [HttpPost]
    public ActionResult DeletePatient(int joinId, int doctorId)
    {
      var joinEntry = _db.DoctorPatientSpecialty.FirstOrDefault(entry => entry.DoctorPatientSpecialtyId == joinId);
      _db.DoctorPatientSpecialty.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Details", new { id = doctorId });
    }

    [HttpPost]
    public ActionResult DeleteSpecialty(int joinId, int doctorId)
    {
      var joinEntry = _db.DoctorPatientSpecialty.FirstOrDefault(entry => entry.DoctorPatientSpecialtyId == joinId);
      _db.DoctorPatientSpecialty.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Details", new { id = doctorId });
    }
  }
}