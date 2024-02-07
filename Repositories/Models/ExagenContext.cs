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

        public virtual DbSet<Document> Documents { get; set; } = null!;
        public virtual DbSet<Exam> Exams { get; set; } = null!;
        public virtual DbSet<Paper> Papers { get; set; } = null!;
        public virtual DbSet<PaperExam> PaperExams { get; set; } = null!;
        public virtual DbSet<Question> Questions { get; set; } = null!;
        public virtual DbSet<QuestionMapping> QuestionMappings { get; set; } = null!;
        public virtual DbSet<QuestionSet> QuestionSets { get; set; } = null!;
        public virtual DbSet<School> Schools { get; set; } = null!;
        public virtual DbSet<Share> Shares { get; set; } = null!;
        public virtual DbSet<Student> Students { get; set; } = null!;
        public virtual DbSet<StudentClass> StudentClasses { get; set; } = null!;
        public virtual DbSet<SubjectSection> SubjectSections { get; set; } = null!;
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
            modelBuilder.Entity<Document>(entity =>
            {
                entity.ToTable("Document");

                entity.Property(e => e.DocumentId)
                    .HasColumnName("DocumentID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Url)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("URL");
            });

            modelBuilder.Entity<Exam>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Exam");

                entity.HasIndex(e => e.CreatedOn, "IX_Exam_CreatedOn")
                    .IsClustered();

                entity.HasIndex(e => new { e.CreatedOn, e.ExamId }, "IX_Exam_ID");

                entity.Property(e => e.ClassId).HasColumnName("ClassID");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.ExamId)
                    .HasColumnName("ExamID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.TestCode).ValueGeneratedOnAdd();

                entity.HasOne(d => d.Class)
                    .WithMany()
                    .HasForeignKey(d => d.ClassId);
            });

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

                entity.Property(e => e.QuestionSetId).HasColumnName("QuestionSetID");
            });

            modelBuilder.Entity<PaperExam>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("PaperExam");

                entity.HasIndex(e => e.ExamId, "IX_PaperExam_ExamID")
                    .IsClustered();

                entity.Property(e => e.ExamId).HasColumnName("ExamID");

                entity.Property(e => e.PaperId).HasColumnName("PaperID");
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

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.SchoolId).HasColumnName("SchoolID");

                entity.Property(e => e.SectionId).HasColumnName("SectionID");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");

                entity.Property(e => e.Subject).HasComputedColumnSql("([dbo].[GetSubjectForSection]([SectionID]))", false);

                entity.HasOne(d => d.School)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.SchoolId);

                entity.HasOne(d => d.Section)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.SectionId);
            });

            modelBuilder.Entity<QuestionMapping>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("QuestionMapping");

                entity.HasIndex(e => e.QuestionSetId, "IX_QuestionMapping_QuestionSetID")
                    .IsClustered();

                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

                entity.Property(e => e.QuestionSetId).HasColumnName("QuestionSetID");
            });

            modelBuilder.Entity<QuestionSet>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("QuestionSet");

                entity.HasIndex(e => e.CreatedOn, "IX_QuestionSet_CreatedOn")
                    .IsClustered();

                entity.HasIndex(e => new { e.CreatedOn, e.QuestionSetId }, "IX_QuestionSet_ID");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.QuestionSetId)
                    .HasColumnName("QuestionSetID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.SchoolId).HasColumnName("SchoolID");

                entity.HasOne(d => d.School)
                    .WithMany()
                    .HasForeignKey(d => d.SchoolId)
                    .OnDelete(DeleteBehavior.Cascade);
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

                entity.Property(e => e.Province).HasMaxLength(50);

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
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

                entity.Property(e => e.QuestionSetId).HasColumnName("QuestionSetID");

                entity.Property(e => e.SchoolId).HasColumnName("SchoolID");

                entity.Property(e => e.ShareId)
                    .HasColumnName("ShareID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.ShareLevel).HasComputedColumnSql("([dbo].[GetShareLevel]([UserID],[SchoolID]))", false);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.School)
                    .WithMany()
                    .HasForeignKey(d => d.SchoolId);

                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
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

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.ClassId);
            });

            modelBuilder.Entity<StudentClass>(entity =>
            {
                entity.HasKey(e => e.ClassId)
                    .HasName("PK__StudentC__CB1927A00BFBB72B");

                entity.ToTable("StudentClass");

                entity.Property(e => e.ClassId)
                    .HasColumnName("ClassID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.SchoolId).HasColumnName("SchoolID");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");

                entity.Property(e => e.TotalStudent).HasComputedColumnSql("([dbo].[CountStudent]([ClassID]))", false);

                entity.HasOne(d => d.School)
                    .WithMany(p => p.StudentClasses)
                    .HasForeignKey(d => d.SchoolId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<SubjectSection>(entity =>
            {
                entity.HasKey(e => e.SectionId)
                    .HasName("PK__SubjectS__80EF08927DDFAD91");

                entity.ToTable("SubjectSection");

                entity.Property(e => e.SectionId)
                    .HasColumnName("SectionID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(500);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.HasIndex(e => e.Email, "UQ__User__A9D105346F3B8237")
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
