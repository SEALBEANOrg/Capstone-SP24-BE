using System;
using System.Collections.Generic;
using System.Text;

namespace ExagenSharedProject
{
    public static class EnumStatus
    {

        //Create Status Enum for Question
        public static Dictionary<int, string> QuestionStatus = new Dictionary<int, string>(
            new KeyValuePair<int, string>[]
            {
                new KeyValuePair<int, string>(0, "Mục đích cá nhân"),
                new KeyValuePair<int, string>(1, "Để bán")
            });

        //Create Status Enum for User
        public static Dictionary<int, string> UserStatus = new Dictionary<int, string>(
            new KeyValuePair<int, string>[]
            {
                new KeyValuePair<int, string>(0, "Ngưng hoạt động"),
                new KeyValuePair<int, string>(1, "Đang hoạt động"),
                new KeyValuePair<int, string>(2, "Đang chờ xác thực"),
                new KeyValuePair<int, string>(3, "Đã xác thực")
            });

        //Create Status Enum for Test
        public static Dictionary<int, string> TestStatus = new Dictionary<int, string>(
            new KeyValuePair<int, string>[]
            {
                new KeyValuePair<int, string>(0, "Bản nháp"),
                new KeyValuePair<int, string>(1, "Chính thức")
            });

        //Create Status Enum for StudentClass
        public static Dictionary<int, string> StudentClassStatus = new Dictionary<int, string>(
            new KeyValuePair<int, string>[]
            {
                new KeyValuePair<int, string>(0, "Ngưng hoạt động"),
                new KeyValuePair<int, string>(1, "Đang hoạt động")
            });

        //Create Option Set for Grade of Subject Section
        public static Dictionary<int, string> Grade = new Dictionary<int, string>(
            new KeyValuePair<int, string>[]
            {
                new KeyValuePair<int, string>( 0, "Khác"),
                new KeyValuePair<int, string>( 1, "Lớp 1"),
                new KeyValuePair<int, string>( 2, "Lớp 2"),
                new KeyValuePair<int, string>( 3, "Lớp 3"),
                new KeyValuePair<int, string>( 4, "Lớp 4"),
                new KeyValuePair<int, string>( 5, "Lớp 5"),
                new KeyValuePair<int, string>( 6, "Lớp 6"),
                new KeyValuePair<int, string>( 7, "Lớp 7"),
                new KeyValuePair<int, string>( 8, "Lớp 8"),
                new KeyValuePair<int, string>( 9, "Lớp 9"),
                new KeyValuePair<int, string>(10, "Lớp 10"),
                new KeyValuePair<int, string>(11, "Lớp 11"),
                new KeyValuePair<int, string>(12, "Lớp 12")
            });

        //Create Option Set for Subject of Subject Section
        public static Dictionary<int, string> Subject = new Dictionary<int, string>(
            new KeyValuePair<int, string>[]
            {
                new KeyValuePair<int, string>(0, "Khác"),
                new KeyValuePair<int, string>(1, "Toán"),
                new KeyValuePair<int, string>(2, "Ngữ Văn/ Tiếng Việt"),
                new KeyValuePair<int, string>(3, "Tiếng Anh"),
                new KeyValuePair<int, string>(4, "Vật Lý"),
                new KeyValuePair<int, string>(5, "Hóa Học"),
                new KeyValuePair<int, string>(6, "Sinh Học"),
                new KeyValuePair<int, string>(7, "Lịch Sử"),
                new KeyValuePair<int, string>(8, "Địa Lý"),
                new KeyValuePair<int, string>(9, "Giáo Dục Công Dân"),
                new KeyValuePair<int, string>(10, "Công Nghệ"),
                new KeyValuePair<int, string>(11, "Tin Học"),
                new KeyValuePair<int, string>(12, "Giáo Dục Quốc Phòng")
            });

        //Create Option Set for Type of Share
        public static Dictionary<int, string> Type = new Dictionary<int, string>(
            new KeyValuePair<int, string>[]
            {
                new KeyValuePair<int, string>(0, "Tính điểm"),
                new KeyValuePair<int, string>(1, "Miễn phí")
            });

        //Create Option Set for PermissionType of Share
        public static Dictionary<int, string> PermissionType = new Dictionary<int, string>(
            new KeyValuePair<int, string>[]
            {
                new KeyValuePair<int, string>(0, "Đọc"),
                new KeyValuePair<int, string>(1, "Đọc, Sửa"),
                new KeyValuePair<int, string>(2, "Đọc, Sửa, Share"),
                new KeyValuePair<int, string>(3, "Đọc, Sửa, Share, Xóa")
            });

        //Create Option Set for ShareLevel of Share
        public static Dictionary<int, string> ShareLevel = new Dictionary<int, string>(
            new KeyValuePair<int, string>[]
            {
                new KeyValuePair<int, string>(0, "Người dùng"),
                new KeyValuePair<int, string>(1, "Trường"),
                new KeyValuePair<int, string>(2, "Cộng đồng")
            });

        //Create Option Set for Difficulty of Question
        public static Dictionary<int, string> Difficulty = new Dictionary<int, string>(
            new KeyValuePair<int, string>[]
            {
                new KeyValuePair<int, string>(0, "Nhận biết"),
                new KeyValuePair<int, string>(1, "Thông hiểu"),
                new KeyValuePair<int, string>(2, "Vận dụng"),
                new KeyValuePair<int, string>(3, "Vận dụng cao")
            });

        //Create Option Set for UserType of User
        public static Dictionary<int, string> PositionID = new Dictionary<int, string>(
            new KeyValuePair<int, string>[]
            {
                new KeyValuePair<int, string>(0, "Admin hệ thống"),
                new KeyValuePair<int, string>(1, "Giáo viên"),
                new KeyValuePair<int, string>(2, "Admin trường"),
                new KeyValuePair<int, string>(3, "Chuyên gia")
            });
    }
}



