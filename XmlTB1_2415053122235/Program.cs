using System;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
namespace XmlTB1_2415053122235
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string line1 = Console.ReadLine();
            if (string.IsNullOrEmpty(line1))
            {
                Console.WriteLine("Lenh khong hop le");
                return;
            }
            String noiDungXml = Console.In.ReadToEnd();
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.LoadXml(noiDungXml);
            }
            catch
            {
                Console.WriteLine("Leng khong hop le");
                return;
            }
            line1 = line1.Trim();
            string command = line1;
            string param = "";
            int spaceIdx = line1.IndexOf('');
            if (spaceIdx > 0)
            {
                command = line1.Substring(0, spaceIdx);
                param = line1.Substring(spaceIdx + 1).Trim;
            }
            XPathNavigator nav = doc.CreateNavigator();
            if (command == "LUONG_TREN")
            {
                XuLyLuongTren(nav, param);
            }
            else if (command == "PHONG")
            {
                XuLyPhong(nav, param);
            }
            else if (command == "DEM")
            {
                XuLyDem(nav);
            }
            else if (command == "XPATH")
            {
                XuLyXPath(nav, param);
            }
            else if (command == "THUONG")
            {
                XuLyThuong(nav);
            }
            else
            {
                Console.WriteLine("Lenh khong hop le");
            }
        }
        static void XuLyLuongTren(XPathNavigator nav, string param)
        {
            if (!double.TryParse(param, NumberStyles.Any, CultureInfo.InvariantCulture, out double x))
            {
                Console.WriteLine("Lenh khong hop le");
                return;
            }
            string xpath = $"//nhanvien[number(luong) > {x.ToString(CultureInfo.InvariantCulture)}]";
            XPathNodeIterator iter = nav.Select(xpath);

            if (iter.Count == 0)
            {
                Console.WriteLine("Khong co nhan vien");
                return;
            }
            while (iter.MoveNext())
            {
                XPathNavigator nv = iter.Current;
                string ma = nv.GetAttribute("ma", "") ?? "";
                XPathNavigator nodeHoten = nv.SelectSingleNode("hoten");
                XPathNavigator nodeLuong = nv.SelectSingleNode("luong");
                string hoten = nodeHoten != null ? nodeHoten.Value : "";
                string luong = nodeLuong != null ? nodeLuong.Value : "";

                Console.WriteLine($"{ma} - {hoten} - {luong}");
            }
        }
    }
    static void XuLyPhong(XPathNavigator nav, string maPhong)
        {
            if (string.IsNullOrEmpty(maPhong))
            {
                Console.WriteLine("Lenh khong hop le");
                return;
            }
            XPathNavigator phongNode = nav.SelectSingleNode($"//phongban[@ma='{maPhong}']");
            if (phongNode == null)
            {
                Console.WriteLine($"Khong co phong {maPhong}");
                return;
            }
            XPathNavigator nvIter = phongNode.Select("nhacvien");
            if (nvIter.Count == 0)
            {
                Console.WriteLine($"Phong {maPhong} chua co nhan vien");
                return;
            }
            while (nvIter.MoveNext())
            {
                XPathNavigator nv = nvIter.Current;
                string ma = nv.GetAttribute("ma", "") ?? "";
                XPathNavigator nodeHoten = nv.SelectSingleNode("hoten");
                string hoten = nodeHoten != null ? nodeHoten.Value : "";

                Console.WriteLine($"{ma} - {hoten}");
            }
        }
        static void XuLyDem(XPathNavigator nav)
        {
            object resPhong = nav.Evaluate("count(//phongban)");
            object resNhanVien = nav.Evaluate("count(//nhanvien)");
            int n = Convert.ToInt32(resPhong);
            int m = Convert.ToInt32(resNhanVien);

            Console.WriteLine($"So phong ban: {n}");
            Console.WriteLine($"So nhan vien: {m}");
        }
        static void XuLyXPath(XPathNavigator nav, string xpathExpr)
        {
            if (string.IsNullOrEmpty(xpathExpr))
            {
                Console.WriteLine("Bieu thuc XPath khong hop le");
                return;
            }
            try
            {
                object result = nav.Evaluate(xpathExpr);

                if (result is XPathNodeIterator iter)
                {
                    if (iter.Count == 0)
                    {
                        Console.WriteLine("Khong co ket qua");
                        return;
                    }
                    while (iter.MoveNext())
                    {
                        XPathNavigator node = iter.Current;
                        Console.WriteLine($"{node.Name}: {node.Value}");
                    }
                }
                else if (result is bool b)
                {
                    Console.WriteLine(b ? "true" : "false");
                }
                else if (result is double d)
                {
                    Console.WriteLine(d.ToString(CultureInfo.InvariantCulture));
                }
                else if (result is string s)
                {
                    Console.WriteLine(s);
                }
                else
                {
                    Console.WriteLine(result.ToString());
                }
            }
            catch
            {
                Console.WriteLine("Bieu thuc XPath khong hop le");
            }
        }
        static void XuLyThuong(XPathNavigator nav)
        {
            XPathNodeIterator iter = nav.Select("//nhanvien");

            bool coNhanVien = false;
            long tongThuong = 0;
            while (iter.MoveNext())
            {
                XPathNavigator nv = iter.Current;
                string namVaoLamStr = nv.GetAttribute("namvaolam", "") ?? "";
                if (int.TryParse(namVaoLamStr, out int namVaoLam))
                {
                    int thamNien = 2026 - namVaoLam;
                    if (thamNien >= 5)
                    {
                        string ma = nv.GetAttribute("ma", "") ?? "";
                        XPathNavigator nodeHoten = nv.SelectSingleNode("hoten");
                        XPathNavigator nodeLuong = nv.SelectSingleNode("luong");
                        string hoten = nodeHoten != null ? nodeHoten.Value : "";
                        long luong = 0;
                        if (nodeLuong != null)
                        {
                            long.TryParse(nodeLuong.Value, out luong);
                        }
                        long thuong = (long)Math.Floor(luong * 1.5);
                        tongThuong += thuong;
                        coNhanVien = true;
                        Console.WriteLine($"{ma} - {hoten}: {thuong}");
                    }
                }
            }
            if (!coNhanVien)
            {
                Console.WriteLine("Khong co nhan vien duoc thuong");
            }
            else
            {
                Console.WriteLine($"Tong thuong: {tongThuong}");
            }
        }
    }
}