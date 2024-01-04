using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

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

        public virtual DbSet<Paper> Papers { get; set; } = null!;
        public virtual DbSet<Question> Questions { get; set; } = null!;
        public virtual DbSet<School> Schools { get; set; } = null!;
        public virtual DbSet<Share> Shares { get; set; } = null!;
        public virtual DbSet<Student> Students { get; set; } = null!;
        public virtual DbSet<StudentClass> StudentClasses { get; set; } = null!;
        public virtual DbSet<SubjectSection> SubjectSections { get; set; } = null!;
        public virtual DbSet<Test> Tests { get; set; } = null!;
        public virtual DbSet<TestResult> TestResults { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server =sealbean.ddns.net,5555; database = Exagen;uid=sa;pwd=741753963741753963; TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Paper>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Paper");

                entity.HasIndex(e => e.CreatedOn, "IX_Paper_CreatedOn")
                    .IsClustered();

                entity.HasIndex(e => new { e.CreatedOn, e.PaperId }, "IX_Paper_ID");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.PaperId)
                    .HasColumnName("PaperID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.TestId).HasColumnName("TestID");
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasKey(e => e.QuestionId)
                    .HasName("pk_question")
                    .IsClustered(false);

                entity.ToTable("Question");

                entity.HasIndex(e => e.Grade, "IX_Question_Grade")
                    .IsClustered();

                entity.Property(e => e.QuestionId)
                    .HasColumnName("QuestionID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.SchoolId).HasColumnName("SchoolID");

                entity.Property(e => e.SectionId).HasColumnName("SectionID");

                entity.Property(e => e.Subject).HasComputedColumnSql("([dbo].[GetSubjectForSection]([SectionID]))", false);
            });

            modelBuilder.Entity<School>(entity =>
            {
                entity.ToTable("School");

                entity.Property(e => e.SchoolId)
                    .HasColumnName("SchoolID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.AdminId).HasColumnName("AdminID");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<Share>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Share");

                entity.HasIndex(e => e.CreatedOn, "IX_Share_CreatedOn")
                    .IsClustered();

                entity.HasIndex(e => new { e.CreatedOn, e.ShareId }, "IX_Share_ID");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

                entity.Property(e => e.SchoolId).HasColumnName("SchoolID");

                entity.Property(e => e.ShareId)
                    .HasColumnName("ShareID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.ShareLevel).HasComputedColumnSql("([dbo].[GetShareLevel]([UserID],[SchoolID]))", false);

                entity.Property(e => e.TestId).HasColumnName("TestID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Question)
                    .WithMany()
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("Student");

                entity.Property(e => e.StudentId)
                    .HasColumnName("StudentID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.ClassId).HasColumnName("ClassID");

                entity.Property(e => e.FullName).HasMaxLength(255);

                entity.Property(e => e.Grade).HasComputedColumnSql("([dbo].[GetGradeForStudent]([ClassID]))", false);
            });

            modelBuilder.Entity<StudentClass>(entity =>
            {
                entity.HasKey(e => e.ClassId)
                    .HasName("PK__StudentC__CB1927A0FC6AC89F");

                entity.ToTable("StudentClass");

                entity.Property(e => e.ClassId)
                    .HasColumnName("ClassID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.SchoolId).HasColumnName("SchoolID");

                entity.Property(e => e.TotalStudent).HasComputedColumnSql("([dbo].[CountStudent]([ClassID]))", false);
            });

            modelBuilder.Entity<SubjectSection>(entity =>
            {
                entity.HasKey(e => e.SectionId)
                    .HasName("PK__SubjectS__80EF08924CD6E0D9");

                entity.ToTable("SubjectSection");

                entity.Property(e => e.SectionId)
                    .HasColumnName("SectionID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(500);
            });

            modelBuilder.Entity<Test>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Test");

                entity.HasIndex(e => e.CreatedOn, "IX_Test_CreatedOn")
                    .IsClustered();

                entity.HasIndex(e => new { e.CreatedOn, e.TestId }, "IX_Test_ID");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.SchoolId).HasColumnName("SchoolID");

                entity.Property(e => e.TestId)
                    .HasColumnName("TestID")
                    .HasDefaultValueSql("(newsequentialid())");
            });

            modelBuilder.Entity<TestResult>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("TestResult");

                entity.HasIndex(e => e.CreatedOn, "IX_TestResult_CreatedOn")
                    .IsClustered();

                entity.HasIndex(e => new { e.CreatedOn, e.ResultId }, "IX_TestResult_ID");

                entity.Property(e => e.ClassId).HasColumnName("ClassID");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.ResultId)
                    .HasColumnName("ResultID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.TestId).HasColumnName("TestID");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.HasIndex(e => e.Email, "UQ__User__A9D10534290506D2")
                    .IsUnique();

                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.FullName).HasMaxLength(255);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.Property(e => e.SchoolId).HasColumnName("SchoolID");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
