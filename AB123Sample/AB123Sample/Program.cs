using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AB123Sample
{
    class Program
    {
        static void Main(string[] args)
        {

            var nameService = new NameService();
            nameService.DoIt();

           StudentService studentService = new StudentService();
            var students = studentService.GetStudentsForAllCourse();
            foreach (var s in students)
            {
                Console.WriteLine(s.Name);
            }

            Console.ReadLine();
        }
    }
}
