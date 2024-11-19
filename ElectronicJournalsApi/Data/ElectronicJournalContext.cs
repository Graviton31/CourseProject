using System;
using System.Collections.Generic;
using ElectronicJournalsApi.Models;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace ElectronicJournalsApi.Data;

public partial class ElectronicJournalContext : DbContext
{
    public ElectronicJournalContext()
    {
    }

    public ElectronicJournalContext(DbContextOptions<ElectronicJournalContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Journal> Journals { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<ScheduleChange> ScheduleChanges { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<UnvisitedStatus> UnvisitedStatuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql(Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.19-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.IdGroup).HasName("PRIMARY");

            entity.ToTable("groups");

            entity.HasIndex(e => e.IdSubject, "fk_groups_subjects1_idx");

            entity.HasIndex(e => e.IdUsers, "fk_groups_users_idx");

            entity.Property(e => e.IdGroup).HasColumnName("id_group");
            entity.Property(e => e.Classroom)
                .HasMaxLength(15)
                .HasColumnName("classroom");
            entity.Property(e => e.IdSubject).HasColumnName("id_subject");
            entity.Property(e => e.IdUsers).HasColumnName("id_users");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
            entity.Property(e => e.StudentCount).HasColumnName("student_count");

            entity.HasOne(d => d.IdSubjectNavigation).WithMany(p => p.Groups)
                .HasForeignKey(d => d.IdSubject)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_groups_subjects1");

            entity.HasOne(d => d.IdUsersNavigation).WithMany(p => p.Groups)
                .HasForeignKey(d => d.IdUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_groups_users");
        });

        modelBuilder.Entity<Journal>(entity =>
        {
            entity.HasKey(e => e.IdJournal).HasName("PRIMARY");

            entity.ToTable("journals");

            entity.HasIndex(e => e.IdGroup, "fk_journals_groups1_idx");

            entity.HasIndex(e => e.IdStudent, "fk_journals_students1_idx");

            entity.HasIndex(e => e.IdUnvisitedStatus, "fk_journals_unvisited_statuses1_idx");

            entity.Property(e => e.IdJournal).HasColumnName("id_journal");
            entity.Property(e => e.IdGroup).HasColumnName("id_group");
            entity.Property(e => e.IdStudent).HasColumnName("id_student");
            entity.Property(e => e.IdUnvisitedStatus).HasColumnName("id_unvisited_status");

            entity.HasOne(d => d.IdGroupNavigation).WithMany(p => p.Journals)
                .HasForeignKey(d => d.IdGroup)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_journals_groups1");

            entity.HasOne(d => d.IdStudentNavigation).WithMany(p => p.Journals)
                .HasForeignKey(d => d.IdStudent)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_journals_students1");

            entity.HasOne(d => d.IdUnvisitedStatusNavigation).WithMany(p => p.Journals)
                .HasForeignKey(d => d.IdUnvisitedStatus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_journals_unvisited_statuses1");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.IdSchedule).HasName("PRIMARY");

            entity.ToTable("schedules");

            entity.HasIndex(e => e.IdGroup, "fk_schedules_groups1_idx");

            entity.Property(e => e.IdSchedule).HasColumnName("id_schedule");
            entity.Property(e => e.EndTime)
                .HasColumnType("time")
                .HasColumnName("end_time");
            entity.Property(e => e.IdGroup).HasColumnName("id_group");
            entity.Property(e => e.StartTime)
                .HasColumnType("time")
                .HasColumnName("start_time");
            entity.Property(e => e.WeekDay)
                .HasColumnType("enum('Пн','Вт','Ср','Чт','Пт','Сб')")
                .HasColumnName("week_day");

            entity.HasOne(d => d.IdGroupNavigation).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.IdGroup)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_schedules_groups1");
        });

        modelBuilder.Entity<ScheduleChange>(entity =>
        {
            entity.HasKey(e => e.IdScheduleChange).HasName("PRIMARY");

            entity.ToTable("schedule_changes");

            entity.HasIndex(e => e.IdGroup, "fk_schedule_changes_groups1_idx");

            entity.Property(e => e.IdScheduleChange).HasColumnName("id_schedule_change");
            entity.Property(e => e.EndTime)
                .HasColumnType("time")
                .HasColumnName("end_time");
            entity.Property(e => e.IdGroup).HasColumnName("id_group");
            entity.Property(e => e.NewDate).HasColumnName("new_date");
            entity.Property(e => e.OldDate).HasColumnName("old_date");
            entity.Property(e => e.StartTime)
                .HasColumnType("time")
                .HasColumnName("start_time");

            entity.HasOne(d => d.IdGroupNavigation).WithMany(p => p.ScheduleChanges)
                .HasForeignKey(d => d.IdGroup)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_schedule_changes_groups1");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.IdStudent).HasName("PRIMARY");

            entity.ToTable("students");

            entity.Property(e => e.IdStudent).HasColumnName("id_student");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
            entity.Property(e => e.ParentPhone)
                .HasMaxLength(15)
                .HasColumnName("parent_phone");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(20)
                .HasColumnName("patronymic");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .HasColumnName("phone");
            entity.Property(e => e.Surname)
                .HasMaxLength(20)
                .HasColumnName("surname");

            entity.HasMany(d => d.IdGroups).WithMany(p => p.IdStudents)
                .UsingEntity<Dictionary<string, object>>(
                    "StudentsHasGroup",
                    r => r.HasOne<Group>().WithMany()
                        .HasForeignKey("IdGroup")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_students_has_groups_groups1"),
                    l => l.HasOne<Student>().WithMany()
                        .HasForeignKey("IdStudent")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_students_has_groups_students1"),
                    j =>
                    {
                        j.HasKey("IdStudent", "IdGroup")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("students_has_groups");
                        j.HasIndex(new[] { "IdGroup" }, "fk_students_has_groups_groups1_idx");
                        j.HasIndex(new[] { "IdStudent" }, "fk_students_has_groups_students1_idx");
                        j.IndexerProperty<int>("IdStudent").HasColumnName("id_student");
                        j.IndexerProperty<int>("IdGroup").HasColumnName("id_group");
                    });
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.IdSubject).HasName("PRIMARY");

            entity.ToTable("subjects");

            entity.Property(e => e.IdSubject).HasColumnName("id_subject");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasColumnName("description");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.FullName)
                .HasMaxLength(40)
                .HasColumnName("full_name");
            entity.Property(e => e.LessonLenght).HasColumnName("lesson_lenght");
            entity.Property(e => e.LessonsCount).HasColumnName("lessons_count");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");

            entity.HasMany(d => d.IdUsers).WithMany(p => p.IdSubjects)
                .UsingEntity<Dictionary<string, object>>(
                    "SubjectsHasUser",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("IdUsers")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_subjects_has_users_users1"),
                    l => l.HasOne<Subject>().WithMany()
                        .HasForeignKey("IdSubject")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_subjects_has_users_subjects1"),
                    j =>
                    {
                        j.HasKey("IdSubject", "IdUsers")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("subjects_has_users");
                        j.HasIndex(new[] { "IdSubject" }, "fk_subjects_has_users_subjects1_idx");
                        j.HasIndex(new[] { "IdUsers" }, "fk_subjects_has_users_users1_idx");
                        j.IndexerProperty<int>("IdSubject").HasColumnName("id_subject");
                        j.IndexerProperty<int>("IdUsers").HasColumnName("id_users");
                    });
        });

        modelBuilder.Entity<UnvisitedStatus>(entity =>
        {
            entity.HasKey(e => e.IdUnvisitedStatus).HasName("PRIMARY");

            entity.ToTable("unvisited_statuses");

            entity.Property(e => e.IdUnvisitedStatus).HasColumnName("id_unvisited_status");
            entity.Property(e => e.Name)
                .HasMaxLength(5)
                .HasColumnName("name");
            entity.Property(e => e.ShortName)
                .HasMaxLength(15)
                .HasColumnName("short_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUsers).HasName("PRIMARY");

            entity.ToTable("users");

            entity.Property(e => e.IdUsers).HasColumnName("id_users");
            entity.Property(e => e.BirthDate).HasColumnName("birth_date");
            entity.Property(e => e.Login)
                .HasMaxLength(20)
                .HasColumnName("login");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(32)
                .HasColumnName("password");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(20)
                .HasColumnName("patronymic");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .HasColumnName("phone");
            entity.Property(e => e.Role)
                .HasColumnType("enum('администратор','руководитель','учитель')")
                .HasColumnName("role");
            entity.Property(e => e.Surname)
                .HasMaxLength(20)
                .HasColumnName("surname");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
