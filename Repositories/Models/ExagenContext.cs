using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace Repositories.Models
{
    public partial class ExagenContext : DbContext
    {
        public ExagenContext()
        {
        }

        public ExagenContext(DbContextOptions<ExagenContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Document> Documents { get; set; } = null!;
        public virtual DbSet<Exam> Exams { get; set; } = null!;
        public virtual DbSet<ExamMark> ExamMarks { get; set; } = null!;
        public virtual DbSet<Paper> Papers { get; set; } = null!;
        public virtual DbSet<PaperSet> PaperSets { get; set; } = null!;
        public virtual DbSet<Question> Questions { get; set; } = null!;
        public virtual DbSet<QuestionInExam> QuestionInExams { get; set; } = null!;
        public virtual DbSet<QuestionInPaper> QuestionInPapers { get; set; } = null!;
        public virtual DbSet<QuestionSet> QuestionSets { get; set; } = null!;
        public virtual DbSet<School> Schools { get; set; } = null!;
        public virtual DbSet<SectionPaperSetConfig> SectionPaperSetConfigs { get; set; } = null!;
        public virtual DbSet<Share> Shares { get; set; } = null!;
        public virtual DbSet<Student> Students { get; set; } = null!;
        public virtual DbSet<StudentClass> StudentClasses { get; set; } = null!;
        public virtual DbSet<Subject> Subjects { get; set; } = null!;
        public virtual DbSet<SubjectSection> SubjectSections { get; set; } = null!;
        public virtual DbSet<Transaction> Transactions { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        private string GetConnectionString()
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            var strConn = config["ConnectionStrings:Exagen"];
            return strConn;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(GetConnectionString());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Document>(entity =>
            {
                entity.ToTable("Document");

                entity.Property(e => e.DocumentId)
                    .HasColumnName("DocumentID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.KeyS3)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<Exam>(entity =>
            {
                entity.ToTable("Exam");

                entity.HasIndex(e => e.TestCode, "UQ__Exam__0B0C35F71C90C039")
                    .IsUnique();

                entity.Property(e => e.ExamId)
                    .HasColumnName("ExamID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.ClassId).HasColumnName("ClassID");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.PaperSetId).HasColumnName("PaperSetID");

                entity.Property(e => e.StudyYear)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasComputedColumnSql("([dbo].[GetStudyYear]([CreatedOn]))", false);

                entity.Property(e => e.SubjectId).HasColumnName("SubjectID");

                entity.Property(e => e.Submitted)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasComputedColumnSql("([dbo].[GetSubmitted]([ExamID]))", false);

                entity.Property(e => e.TestCode).ValueGeneratedOnAdd();

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Exams)
                    .HasForeignKey(d => d.ClassId);

                entity.HasOne(d => d.PaperSet)
                    .WithMany(p => p.Exams)
                    .HasForeignKey(d => d.PaperSetId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.Exams)
                    .HasForeignKey(d => d.SubjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ExamMark>(entity =>
            {
                entity.HasKey(e => e.ExamMarkId)
                    .HasName("pk_exammark")
                    .IsClustered(false);

                entity.ToTable("ExamMark");

                entity.HasIndex(e => e.CreatedOn, "IX_ExamMark_CreatedOn")
                    .IsClustered();

                entity.Property(e => e.ExamMarkId)
                    .HasColumnName("ExamMarkID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.AnswersSelected).IsUnicode(false);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ExamId).HasColumnName("ExamID");

                entity.Property(e => e.Mark).HasColumnType("decimal(4, 2)");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Status).HasComputedColumnSql("([dbo].[GetStatus]([PaperCode],[AnswersSelected]))", false);

                entity.Property(e => e.StudentCode).HasComputedColumnSql("([dbo].[GetStudentCode]([StudentID]))", false);

                entity.Property(e => e.StudentId).HasColumnName("StudentID");

                entity.HasOne(d => d.Exam)
                    .WithMany(p => p.ExamMarks)
                    .HasForeignKey(d => d.ExamId);

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.ExamMarks)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Paper>(entity =>
            {
                entity.HasKey(e => e.PaperId)
                    .HasName("pk_paper")
                    .IsClustered(false);

                entity.ToTable("Paper");

                entity.HasIndex(e => e.CreatedOn, "IX_Paper_CreatedOn")
                    .IsClustered();

                entity.Property(e => e.PaperId)
                    .HasColumnName("PaperID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.KeyS3)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.PaperSetId).HasColumnName("PaperSetID");

                entity.HasOne(d => d.PaperSet)
                    .WithMany(p => p.Papers)
                    .HasForeignKey(d => d.PaperSetId);
            });

            modelBuilder.Entity<PaperSet>(entity =>
            {
                entity.ToTable("PaperSet");

                entity.Property(e => e.PaperSetId)
                    .HasColumnName("PaperSetID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.KeyS3)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasKey(e => e.QuestionId)
                    .HasName("pk_question")
                    .IsClustered(false);

                entity.ToTable("Question");

                entity.HasIndex(e => new { e.Grade, e.QuestionId }, "IDX_QUESTIONID")
                    .IsClustered();

                entity.Property(e => e.QuestionId)
                    .HasColumnName("QuestionID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CorrectAnswer)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.QuestionSetId).HasColumnName("QuestionSetID");

                entity.Property(e => e.SectionId).HasColumnName("SectionID");

                entity.Property(e => e.SubjectId)
                    .HasColumnName("SubjectID")
                    .HasComputedColumnSql("([dbo].[GetSubjectOfSection]([SectionID]))", false);

                entity.HasOne(d => d.QuestionSet)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.QuestionSetId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Section)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.SectionId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<QuestionInExam>(entity =>
            {
                entity.HasKey(e => new { e.ExamId, e.QuestionId })
                    .HasName("pk_questioninexam");

                entity.ToTable("QuestionInExam");

                entity.Property(e => e.ExamId).HasColumnName("ExamID");

                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

                entity.HasOne(d => d.Exam)
                    .WithMany(p => p.QuestionInExams)
                    .HasForeignKey(d => d.ExamId);

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.QuestionInExams)
                    .HasForeignKey(d => d.QuestionId);
            });

            modelBuilder.Entity<QuestionInPaper>(entity =>
            {
                entity.HasKey(e => new { e.PaperId, e.QuestionId })
                    .HasName("pk_questioninpaper");

                entity.ToTable("QuestionInPaper");

                entity.Property(e => e.PaperId).HasColumnName("PaperID");

                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

                entity.HasOne(d => d.Paper)
                    .WithMany(p => p.QuestionInPapers)
                    .HasForeignKey(d => d.PaperId);

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.QuestionInPapers)
                    .HasForeignKey(d => d.QuestionId);
            });

            modelBuilder.Entity<QuestionSet>(entity =>
            {
                entity.HasKey(e => e.QuestionSetId)
                    .HasName("pk_questionset")
                    .IsClustered(false);

                entity.ToTable("QuestionSet");

                entity.HasIndex(e => e.CreatedOn, "IX_QuestionSet_CreatedOn")
                    .IsClustered();

                entity.Property(e => e.QuestionSetId)
                    .HasColumnName("QuestionSetID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.Grade).HasComputedColumnSql("([dbo].[GetGradeOfSubject]([SubjectID]))", false);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.StudyYear)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasComputedColumnSql("([dbo].[GetStudyYear]([CreatedOn]))", false);

                entity.Property(e => e.SubjectId).HasColumnName("SubjectID");

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.QuestionSets)
                    .HasForeignKey(d => d.SubjectId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<School>(entity =>
            {
                entity.ToTable("School");

                entity.Property(e => e.SchoolId)
                    .HasColumnName("SchoolID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.City).HasMaxLength(50);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.Province).HasMaxLength(50);
            });

            modelBuilder.Entity<SectionPaperSetConfig>(entity =>
            {
                entity.HasKey(e => new { e.Difficulty, e.PaperSetId, e.SectionId })
                    .HasName("pk_sectionexam");

                entity.ToTable("SectionPaperSetConfig");

                entity.Property(e => e.PaperSetId).HasColumnName("PaperSetID");

                entity.Property(e => e.SectionId).HasColumnName("SectionID");

                entity.HasOne(d => d.PaperSet)
                    .WithMany(p => p.SectionPaperSetConfigs)
                    .HasForeignKey(d => d.PaperSetId);

                entity.HasOne(d => d.Section)
                    .WithMany(p => p.SectionPaperSetConfigs)
                    .HasForeignKey(d => d.SectionId);
            });

            modelBuilder.Entity<Share>(entity =>
            {
                entity.HasKey(e => new { e.ShareId });

                entity.ToTable("Share");

                entity.HasIndex(e => e.CreatedOn, "IX_Share_CreatedOn")
                    .IsClustered();

                entity.HasIndex(e => new { e.CreatedOn, e.ShareId }, "IX_Share_ID");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.QuestionSetId).HasColumnName("QuestionSetID");

                entity.Property(e => e.ShareId)
                    .HasColumnName("ShareID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.QuestionSet)
                    .WithMany()
                    .HasForeignKey(d => d.QuestionSetId);

                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("Student");

                entity.Property(e => e.StudentId)
                    .HasColumnName("StudentID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.ClassId).HasColumnName("ClassID");

                entity.Property(e => e.Dob)
                    .HasColumnType("date")
                    .HasColumnName("DOB");

                entity.Property(e => e.FullName).HasMaxLength(255);

                entity.Property(e => e.Grade).HasComputedColumnSql("([dbo].[GetGradeForStudent]([ClassID]))", false);

                entity.Property(e => e.ParentPhoneNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.ClassId);
            });

            modelBuilder.Entity<StudentClass>(entity =>
            {
                entity.HasKey(e => e.ClassId)
                    .HasName("PK__StudentC__CB1927A02EAA6880");

                entity.ToTable("StudentClass");

                entity.Property(e => e.ClassId)
                    .HasColumnName("ClassID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");

                entity.Property(e => e.StudyYear)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasComputedColumnSql("([dbo].[GetStudyYear]([CreatedOn]))", false);

                entity.Property(e => e.TotalStudent).HasComputedColumnSql("([dbo].[CountStudent]([ClassID]))", false);
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.ToTable("Subject");

                entity.Property(e => e.SubjectId)
                    .HasColumnName("SubjectID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<SubjectSection>(entity =>
            {
                entity.HasKey(e => e.SectionId)
                    .HasName("PK__SubjectS__80EF0892297942A8");

                entity.ToTable("SubjectSection");

                entity.Property(e => e.SectionId)
                    .HasColumnName("SectionID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.SubjectId).HasColumnName("SubjectID");

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.SubjectSections)
                    .HasForeignKey(d => d.SubjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.TransactionId);

                entity.ToTable("Transaction");

                entity.HasIndex(e => e.CreatedOn, "IX_TRANSACTION_CreatedOn")
                    .IsClustered();

                entity.HasIndex(e => new { e.CreatedOn, e.TransactionId }, "IX_TRANSACTION_ID");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.TransactionCode).HasMaxLength(255);

                entity.Property(e => e.TransactionId)
                    .HasColumnName("TransactionID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Type).HasColumnName("TYPE");

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.HasIndex(e => e.Email, "UQ__User__A9D1053485A60A7F")
                    .IsUnique();

                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.FullName).HasMaxLength(255);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.SchoolId).HasColumnName("SchoolID");

                entity.HasOne(d => d.School)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.SchoolId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
