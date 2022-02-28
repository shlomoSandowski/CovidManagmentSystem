using CovidManagementSystem.Controllers;
namespace CovidManagementSystem.Entities
{

    public class CovidSystemManager : ICovidSystemManager
    {
        public PatientManager patientManager = new PatientManager();
        public List<PotentialPatient> PotentialPatients = new List<PotentialPatient>();
        public List<LabTestResult> LabTestResult = new List<LabTestResult>();
        public CovidStatistics statistic = new CovidStatistics();
     

        public NewPatientResponse AddPatient(NewPatientRequest newPatient) 
        {
            statistic.addInfected(newPatient.address.city,true);
            return patientManager.AddPatient(newPatient);
        }
        public List<Patient> GetPatients() 
        { 
            return patientManager.GetPatients();
        }
        public void AddPatientRoute(string id, NewPatientVisitedSite newSite)
        { 
            patientManager.AddPatientVisitedSite(id, newSite);

        }
        public List<NewPatientVisitedSite> GetPatientRoute(string id)
        { 
            return patientManager.GetPatientRoute(id);
        }
        public void AddPatientEncounter(string id, NewPatientEncounter encounter)
        {
            PotentialPatient encounterPatient = new PotentialPatient(encounter.firstName,encounter.lastName,encounter.phoneNumber,id);
            patientManager.AddPatientEncounter(id, encounterPatient);
            PotentialPatients.Add(encounterPatient);
            statistic.isolated++;
        }
        public List<PatientEncounter> GetPatientEncounters(string id)
        { 
            return patientManager.GetPatientEncounters(id);
        }
        //full
        public PatientMedicalFile GetPatient(string id) 
        { 
            return patientManager.GetMedicalFile(id); 
        }
        public List<PatientEncounter> GetNewSick(DateTime since)
        {
            List<PatientEncounter> newSick = new List<PatientEncounter>();
          
            PotentialPatients.ForEach(potential =>
            { if (potential != null && potential.IsCovidPositive() && (since - potential.GetIsolationDate()).Days >= 0)
                    newSick.Add(new PatientEncounter(potential, patientManager.GetPatient(potential.GetInfectedByPatientID())));
            });
            newSick.AddRange(patientManager.GetNewSick(since));

            return newSick;

        }
        public List<PatientEncounter> GetPotentialPatients()
        {
            List<PatientEncounter> potentialPatients = new List<PatientEncounter>();
            PotentialPatients.ForEach(
                (potential) => potentialPatients.Add(new PatientEncounter(potential, patientManager.GetPatient(potential.GetInfectedByPatientID()))));
            return potentialPatients;
        }
        public List<PatientEncounter> GetIsolated()
        {
            List<PatientEncounter> isolated = new List<PatientEncounter>();
            PotentialPatients.ForEach(
                (potential) => {if (potential.IsIsolated())
                        isolated.Add(new PatientEncounter(potential, patientManager.GetPatient(potential.GetInfectedByPatientID())));
                });
            isolated.AddRange(patientManager.GetIsolatedPatients());
            return isolated;
        }
        public NewPatientResponse UpdatePatient(string id,NewPatientRequest newPatient)
        {

            PotentialPatient upgrade = PotentialPatients.FirstOrDefault(potential=>potential.potentialPatientID == id);
            if (upgrade != null)
            {
                if (newPatient.firstName == upgrade.firstName && newPatient.lastName == upgrade.lastName && newPatient.phoneNumber == upgrade.phoneNumber)
                    PotentialPatients.Remove(upgrade);
                    return patientManager.AddPatient(upgrade, newPatient.govtID, newPatient.birthDate, newPatient.email, newPatient.address, newPatient.houseResidentAmount);
            }
            throw new Exception($"no potential patient with id {id}");
               
        } 
        public NewPatientResponse AddLabTest(LabTestResult labTestResult) 
        {
            string city = null;
            IPotentialPatient toAdd = patientManager.GetPatient(labTestResult.patientID);
            if (toAdd != null)
                city = ((Patient)toAdd).address.city;
            if (toAdd == null) 
                toAdd = PotentialPatients.Where(potential =>potential.potentialPatientID == labTestResult.patientID).FirstOrDefault();
            if(toAdd != null)
            {
               bool isCovidPositive = toAdd.IsCovidPositive();
               bool isIsolated = toAdd.IsIsolated();
               toAdd.AddLabResults(labTestResult);
                if (isCovidPositive && !toAdd.IsCovidPositive())
                    statistic.addHealed(city,toAdd.IsIsolated()&&!isIsolated);
                if (!isCovidPositive && toAdd.IsCovidPositive())
                    statistic.addInfected(city,toAdd.IsIsolated());

                return new NewPatientResponse { patientID = labTestResult.patientID };
            }
            throw new Exception($"no potential patient with id {labTestResult.patientID}");

        }
        public CovidStatistics GetStatistic() 
        {
            return statistic;
        }
        public DateTime GetIsolationDate(string id)
        {
            IPotentialPatient patient = patientManager.GetPatient(id);
            return patient.GetIsolationDate();  
        }

    }
}
