using System;
using System.Collections.Generic;
using System.Text;

namespace ExagenSharedProject;

public class OptionSet
{
    public class Share
    {
        public class Type
        {
            public static int ForSell = 0;
            public static int Private = 1;
            public static int Public = 2;
        }
    }

    public class Question
    {
        public class Difficulty
        {
            public static int NB = 0;
            public static int TH = 1;
            public static int VD = 2;
            public static int VDC = 3;
        }
    }

    public class User
    {
        public class UserType
        {
            public static int Admin = 0;
            public static int Teacher = 1;
            public static int Expert = 2;
        }
    }

    public class Transaction
    {
        public class Type
        {
            public static int BuyCoin = 0;
            public static int CreatePaper = 1;
            public static int CalculateMark = 2;
            public static int BuyQuestionSet = 3;
            public static int SellQuestionSet = 4;
            public static int FirstSignIn = 5;
            public static int PublicQuestionSet = 6;
        }
    }

    public class Grade
    {
        public static int Other = 0;
        public static int Grade1 = 1;
        public static int Grade2 = 2;
        public static int Grade3 = 3;
            public static int Grade4 = 4;
        public static int Grade5 = 5;
        public static int Grade6 = 6;
        public static int Grade7 = 7;
        public static int Grade8 = 8;
        public static int Grade9 = 9;
        public static int Grade10 = 10;
        public static int Grade11 = 11;
        public static int Grade12 = 12;
    }

    public class Subject
    {
        public static int Other = 0;
        public static int Math = 1;
        public static int Literature = 2;
        public static int English = 3;
        public static int Physics = 4;
        public static int Chemistry = 5;
        public static int Biology = 6;
        public static int History = 7;
        public static int Geography = 8;
        public static int CivicEducation = 9;
        public static int Informatics = 10;
        public static int Information = 11;
        public static int MilitaryEducation = 12;
    }
}

public class Status
{
    public class User
    {
        public static int Inactive = 0;
        public static int Active = 1;
    }

    public class QuestionSet
    {
        public static int Inactive = 0;
        public static int Published = 1;
        public static int Public = 2;
    }

    public class StudentClass
    {
        public static int Inactive = 0;
        public static int Active = 1;
    }

    public class Share
    {
        public static int Pending = 0;
        public static int Approved = 1;
        public static int Rejected = 2;
    }

    public class  Exam
    {
        public static int NotCalculated = 0;
        public static int Calculated = 1;
    }

    public class ExamMark
    {
        public static int Pending = 0;
        public static int Submitted = 1;
        public static int NotSubmitted = 2;
    }
}