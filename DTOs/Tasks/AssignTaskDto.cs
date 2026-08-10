namespace TaskMngBack.DTOs.Tasks
{
    public class AssignTaskDto
    {
        // Gönderilen liste yeni atanan listesinin tamamı (boş = tüm atamaları kaldır)
        public List<int> AssignedUserIds { get; set; } = new();
    }
}
