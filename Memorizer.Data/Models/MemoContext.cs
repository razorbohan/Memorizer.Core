using Microsoft.EntityFrameworkCore;

namespace Memorizer.Data.Models
{
    public class MemoContext : DbContext
    {
        public MemoContext(DbContextOptions<MemoContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Memo> Memos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Memo>(entity =>
            {
                entity.ToTable("Memos");

                entity.Property(e => e.Question)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasIndex(e => e.Question)
                    .HasName("Q_Unique")
                    .IsUnique();

                entity.Property(e => e.Answer)
                    .IsRequired()
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                //entity.Property(e => e.RepeatDate)
                //    .HasColumnType("datetime");
            });
        }
    }
}
