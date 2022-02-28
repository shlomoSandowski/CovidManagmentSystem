using CovidManagementSystem.Controllers;
namespace CovidManagementSystem.Entities
{
    public interface ICovidSystemManager
    {
        public NewPatientResponse AddPatient(NewPatientRequest newPatient);
        public List<Patient> GetPatients();
        public void AddPatientRoute(string id,NewPatientVisitedSite newSite);
        public List<NewPatientVisitedSite> GetPatientRoute(string id);
        public void AddPatientEncounter(string id, NewPatientEncounter encounter);
        public List<PatientEncounter> GetPatientEncounters(string id);
        public PatientMedicalFile GetPatient(string id);//full
        public List<PatientEncounter> GetNewSick(DateTime dateTime);
        public List<PatientEncounter> GetPotentialPatients();
        public List<PatientEncounter> GetIsolated();
        public NewPatientResponse UpdatePatient(string id, NewPatientRequest newPatient);
        public NewPatientResponse AddLabTest(LabTestResult labTestResult);
        public CovidStatistics GetStatistic();
        public DateTime GetIsolationDate(string id);

    }
}