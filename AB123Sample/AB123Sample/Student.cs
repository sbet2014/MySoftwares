using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AB123Sample
{
    public class Student
    {
        public Student()
        {
            StudentCourse = new HashSet<StudentCourse>();
        }

        public int StudentId { get; set; }

        public string Name { get; set; }

        public IEnumerable<StudentCourse> StudentCourse { get; set; }
    }

    public class Course
    {
        public Course()
        {
            StudentCourse = new HashSet<StudentCourse>();
        }

        public int CourseId { get; set; }

        public string Name { get; set; }

        public IEnumerable<StudentCourse> StudentCourse { get; set; }
    }

    public class StudentCourse
    {
        public int Id { get; set; }

        public int CourseId { get; set; }

        public Course Course { get; set; }

        public int StudentId { get; set; }

        public Student Student { get; set; }
    }

    public class StudentService
    {

        public StudentService()
        {
            var students = Students;
            var courses = Courses;
            var studentCourses = StudentCourses;
        }

        public ICollection<Student> Students
        {
            get
            {
                return new List<Student>()
                {
                    new Student { StudentId=1, Name="小明"},
                    new Student { StudentId=2, Name="小李" },
                    new Student { StudentId=3, Name="小郑" },
                    new Student { StudentId=4, Name="小王" }
                };
            }
        }

        public ICollection<Course> Courses
        {
            get
            {
                return new List<Course>()
                {
                    new Course { CourseId=1, Name="语文" },
                    new Course { CourseId=2, Name="数学" },
                    new Course { CourseId=3, Name="物理" }
                };
            }
        }

        public ICollection<StudentCourse> StudentCourses
        {
            //小明选了所有课程,小李选了2门课程,小郑选了1门课程,,小王选了0门课程
            get
            {
                return new List<StudentCourse>()
                {
                    new StudentCourse { Id=1, StudentId=1, Student=Students.First(s=>s.StudentId==1), CourseId=1, Course=Courses.First(c=>c.CourseId==1)},
                    new StudentCourse { Id=2, StudentId=1, Student=Students.First(s=>s.StudentId==1), CourseId=2, Course=Courses.First(c=>c.CourseId==2)},
                    new StudentCourse { Id=3, StudentId=1, Student=Students.First(s=>s.StudentId==1), CourseId=3, Course=Courses.First(c=>c.CourseId==3)},

                    new StudentCourse { Id=4, StudentId=2, Student=Students.First(s=>s.StudentId==2), CourseId=1, Course=Courses.First(c=>c.CourseId==1)},
                    new StudentCourse { Id=5, StudentId=2, Student=Students.First(s=>s.StudentId==2), CourseId=2, Course=Courses.First(c=>c.CourseId==2)},

                    new StudentCourse { Id=6, StudentId=3, Student=Students.First(s=>s.StudentId==3), CourseId=1, Course=Courses.First(c=>c.CourseId==1)},
                };
            }
        }

        public IEnumerable<Student> GetStudentsForAllCourse()
        {
            return from x in Students.Where(s =>
                   {
                       //return x.StudentId == s.StudentId && !Courses.Any(c =>
                       // {
                       //     return (from sc in StudentCourses
                       //             where sc.StudentId == s.StudentId && sc.CourseId == c.CourseId
                       //             select sc)
                       //             .Count() > 0;
                       // });
                       var exists = (Courses.Select(c => c.CourseId).SequenceEqual(from sc in StudentCourses
                                                                            where sc.StudentId == s.StudentId
                                                                            select sc.CourseId));

                       return exists;
                   })
                   select x;
        }
    }
}
