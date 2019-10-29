using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AB123Sample
{
    public class NameService
    {
        public void DoIt()
        {
            string[] names = new string[] { "A-1", "A-2", "A-3", "A-4", "B-1", "B-2", "B-3" };

            //var result11 = names.OfType<int>().ToList();

            var result1 = (from n in names
                           select int.Parse(n.Last().ToString())).ToList().Distinct();

            var result2 = from n in names
                          group n by n.First().ToString() into mg
                          let m = new { Month = mg.Key, Orders = mg }
                          where m.Orders.All(z =>
                          {
                              return ((from n in names
                                       select int.Parse(n.Last().ToString())).ToList().Distinct()).FirstOrDefault(x =>
                                       {
                                           return x == int.Parse(z.Last().ToString());
                                       }) != 0;
                          })
                          select m.Month;

            //var result3 = (from n in names
            //               select n).Any(x =>
            //                            {
            //                                (from n in names
            //                                 select int.Parse(n.Last().ToString())).ToList().Distinct().Any(y =>
            //                                                                                               {
            //                                                                                                   return names.All(z =>
            //                                                                                                                   {
            //                                                                                                                       return z == x && y == 1;
            //                                                                                                                    });

            //                                                                                               });

            //                               return false;
            //                            });


            //var result4 = from a in names.Where(xx =>
            //                                    {
            //                                        return names.Any(x =>
            //                                               {
            //                                                   return xx == x && (from b in names
            //                                                                      select int.Parse(b.Last().ToString())).ToList().Distinct().Any(y =>
            //                                                                                                                                    {
            //                                                                                                                                        return (from c in names
            //                                                                                                                                                select c).Any(z =>
            //                                                                                                                                                              {
            //                                                                                                                                                                  int index = int.Parse(x.Last().ToString());
            //                                                                                                                                                                  return x == z && y == index;
            //                                                                                                                                                              });
            //                                                                                                                                    });
            //                                               });
            //                                    })
            //              select a;

            var result5 = from a in names
                          where names.Any(x =>
                          {
                              var exists1 = (((from b in names
                                               select int.Parse(b.Last().ToString())).ToList().Distinct()).Any(y =>
                                               {
                                                   var name =  (from c in names
                                                                  where c.First() == x.First() && int.Parse(c.Last().ToString()) == y
                                                                  select c).ToList();
                                                   var exists2= name.FirstOrDefault() == null;
                                                   //var exists2 = (from c in names
                                                   //               where c.First() == x.First() && int.Parse(c.Last().ToString()) == y
                                                   //               select c)
                                                   //              .FirstOrDefault() == null;
                                                   return exists2;
                                               }));
                              return !exists1;
                          })
                          select a;

            var result6 = from a in names
                          where !names.Any(x =>
                          {
                              return result1.Any(y =>
                              {
                                  return (from c in names
                                          where c.First().ToString() == x.First().ToString() && int.Parse(c.Last().ToString()) == y
                                          select c)
                                          .Count() > 0;
                              });

                          })
                          select a.First();

            var result4 = from a in names.Where(x =>
            {
                var exists = (from n in names
                              select int.Parse(n.Last().ToString())).ToList().Distinct()
                               .SequenceEqual(from b in names
                                              where b.First().ToString() == x.First().ToString()
                                              select int.Parse(b.Last().ToString()));

                return exists;
            })
                          select a;
            //var result4 = from a in names
            //              where names.Any(x =>
            //              {
            //                  return !(from c in names
            //                           select c).Any(y =>
            //                          {
            //                              return ((from b in names
            //                                       select int.Parse(b.Last().ToString())).ToList().Distinct()).Any(z =>
            //                                         {
            //                                          int index = int.Parse(y.Last().ToString());
            //                                          return x == y && index == z;
            //                                      });
            //                          });

            //              })
            //              select a;
            for (int i = 0; i < result1.Count(); i++)
            {
                Console.WriteLine(result1.ElementAt(i));
            }

            for (int i = 0; i < result5.Count(); i++)
            {
                Console.WriteLine(result5.ElementAt(i));
            }

            //foreach (var i in result4)
            //{
            //    Console.WriteLine(i);
            //}
        }
    }
}
