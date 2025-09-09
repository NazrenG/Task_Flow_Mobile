namespace Task_Flow.WebAPI.Dtos
{
    public class PutProjectDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        //public string? CreatedById { get; set; }//UserId
       
        public string? Status { get; set; }
        //public bool IsCompleted { get; set; }
        public string? Color { get; set; }
    }
}
