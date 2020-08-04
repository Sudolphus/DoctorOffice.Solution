namespace DoctorOffice.Models
{
  public class DoctorPatientSpecialty
  {
    public int DoctorPatientSpecialtyId { get; set; }
    public int DoctorId { get; set; }
    public int? PatientId { get; set; }
    public int? SpecialtyId { get; set; }
    public Doctor Doctor { get; set; }
    public Patient Patient { get; set; }
    public Specialty Specialty { get; set; }
  }
}