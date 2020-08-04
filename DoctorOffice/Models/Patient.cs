using System;
using System.Collections.Generic;

namespace DoctorOffice.Models
{
  public class Patient
  {
    public int PatientId { get; set; }
    public string Name { get; set; }
    public DateTime Birthday { get; set; }
    public virtual ICollection<DoctorPatientSpecialty> Doctors { get; set; }

    public Patient()
    {
      this.Doctors = new HashSet<DoctorPatientSpecialty>();
    }
  }
}