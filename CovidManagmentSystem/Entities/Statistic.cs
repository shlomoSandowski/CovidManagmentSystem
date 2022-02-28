namespace CovidManagementSystem.Entities
{
    public class CovidStatistics
    {
        public int infected { get; set; }
        public int healed { get; set; }
        public int isolated { get; set; }

        public List<CityCovidStats> cityStatistics { get; set; }
        public CovidStatistics()
        {
            cityStatistics = new List<CityCovidStats>();
            infected = 0;
            healed = 0;
            isolated = 0;
        }

        public void addCity(string cityName,int infected)
        {
            cityStatistics.Add(new CityCovidStats(cityName,infected));
        }

        public void addInfected(string cityName, bool isolated)
        {
            var stat = cityStatistics.Find(cityStat => cityStat.city == cityName);
            if (stat != null)
                stat.infected++;
            else
                cityStatistics.Add(new CityCovidStats(cityName));

            infected++;
            if(isolated)
                this.isolated++;

        }

        public void addHealed(string cityName,bool isolated)
        {
            var stat = cityStatistics.Find(cityStat => cityStat.city == cityName ) ;
            if (stat != null) 
            {   
                stat.infected--;
                infected--;
                if(!isolated)
                    this.isolated--;
                healed++;
            }
             

        }
    }

    public class CityCovidStats
    {
        private string _city { get; init; }
        public string city { get { return this._city; } }
        public int infected { get; set; }
        public CityCovidStats(string city,int infected = 1)
        {
            this._city = city;
            this.infected = infected; 
        }
    }
}
