using System.ComponentModel.DataAnnotations;
namespace CovidManagementSystem.Entities
{
    public class PatientMedicalFile
    {
        [Required]
        public Patient patientDetails{get;init;}
        [Required,Range(0,1)]
        public bool isCovidPositive { get; init; }
        public List<LabTestResult>? labResults { get; init; }
        public PatientMedicalFile(Patient patientDetails, bool isCovidPositive, List<LabTestResult> labTestResults)
        {
            this.patientDetails = patientDetails;
            this.isCovidPositive = isCovidPositive;
            this.labResults = labTestResults;
        }

    }

}


