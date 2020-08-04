using System.Collections.Generic;

namespace DoctorOffice.Models
{
  public class Doctor
  {
    public int DoctorId { get; set; }
    public string Name { get; set; }
    public virtual ICollection<DoctorPatientSpecialty> PatientsSpecialties { get; set; }

    public Doctor()
    {
      this.PatientsSpecialties = new HashSet<DoctorPatientSpecialty>();
    }
  }
}