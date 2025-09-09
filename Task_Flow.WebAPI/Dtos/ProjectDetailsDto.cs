namespace Task_Flow.WebAPI.Dtos
{
    public class ProjectDetailsDto
    {
        public string ProjectName {  get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public string Status { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Color { get; set; }
        public string OwnerName {  get; set; }
        public string OwnerEmail { get; set; }
        public string Occupation { get; set; }
        public string Gender { get; set; }
        public string Birthday { get; set; }
        public string Phone { get; set; }
        public bool? IsOnline { get; set; }
    }
}
