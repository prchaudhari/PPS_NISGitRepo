using System.Data.Entity;

namespace NedbankRepository
{
    public class NedbankDbInitializer : CreateDatabaseIfNotExists<NedbankDbContext>
    {
        protected override void Seed(NedbankDbContext context)
        {
            //IList<Grade> grades = new List<Grade>();

            //grades.Add(new Grade() { GradeName = "Grade 1", Section = "A" });
            //grades.Add(new Grade() { GradeName = "Grade 1", Section = "B" });
            //grades.Add(new Grade() { GradeName = "Grade 1", Section = "C" });
            //grades.Add(new Grade() { GradeName = "Grade 2", Section = "A" });
            //grades.Add(new Grade() { GradeName = "Grade 3", Section = "A" });

            //context.Grades.AddRange(grades);

            base.Seed(context);
        }
    }
}
