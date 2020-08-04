using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DoctorOffice.Models;

namespace DoctorOffice.Controllers
{
  public class PatientsController : Controller
  {
    private readonly DoctorOfficeContext _db;
    public PatientsController(DoctorOfficeContext db)
    {
      _db = db;
    }

    public ActionResult Index(string patientName) //same search filtering functionality as used on line 20 in doctors controllers using IQueryable
    {
      IQueryable<Patient> patientQuery = _db.Patients;
      if (!string.IsNullOrEmpty(patientName))
      {
        Regex search = new Regex(patientName, RegexOptions.IgnoreCase);
        patientQuery = patientQuery.Where(patients => search.IsMatch(patients.Name));
      }
      IEnumerable<Patient> model = patientQuery.ToList().OrderBy(patients => patients.Name);
      return View(model);
    }

    public ActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public ActionResult Create(Patient patient)
    {
      _db.Patients.Add(patient);
      _db.SaveChanges();
      return RedirectToAction("Details", new { id = patient.PatientId });
    }

    public ActionResult Details(int id)
    {
      var thisPatient = _db.Patients
        .Include(patients => patients.Doctors)
          .ThenInclude(join => join.Doctor)
        .FirstOrDefault(patients => patients.PatientId == id);
      return View(thisPatient);
    }

    public ActionResult Edit(int id)
    {
      var thisPatient = _db.Patients.FirstOrDefault(patients => patients.Id == id);
      ViewBag.DoctorId = new SelectList(_db.Doctors, "DoctorId", "Name");
      return View(thisPatient);
    }

    [HttpPost]
    public ActionResult Edit(Patient patient, int DoctorId)
    {
      if (DoctorId != 0)
      {
        _db.DoctorPatientSpecialty.Add(new DoctorPatientSpecialty() { DoctorId = DoctorId, PatientId = patient.PatientId });
      }
      _db.Entry(patient).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Details", new { id = patient.PatientId });
    }

    public ActionResult Delete(int id)
    {
      var thisPatient = _db.Patients.FirstOrDefault(patients => patients.PatientId == id);
      return View(thisPatient);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisPatient = _db.Patients.FirstOrDefault(patients => patients.PatientId == id);
      _db.Remove(thisPatient);
      return RedirectToAction("Index");
    }
  }
}