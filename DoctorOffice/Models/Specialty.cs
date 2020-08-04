using System.Collections.Generic;

namespace DoctorOffice.Models
{
  public class Specialty
  {
    public int SpecialtyId { get; set; }
    public string Type { get; set; }
    public ICollection<DoctorPatientSpecialty> Doctors { get; set; }

    public Specialty()
    {
      this.Doctors = new HashSet<DoctorPatientSpecialty>();
    }
  }
}