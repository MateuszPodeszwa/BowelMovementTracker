using Microsoft.EntityFrameworkCore;

namespace BowelMovementTracker.Data;

public class BowelMovementTrackerContext(DbContextOptions<BowelMovementTrackerContext> options) : DbContext(options)
{
    public DbSet<User> User { get; set; } = null!;
    public DbSet<Diary> Diary { get; set; } = null!;
    public DbSet<Log> Log { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // User Object
        modelBuilder.Entity<User>(entity =>
        {
            // ID
            entity.HasKey(e => e.UserIdentifier);
            entity.Property(e => e.UserIdentifier).ValueGeneratedOnAdd().HasDefaultValueSql("newsequentialid()");
            entity.HasIndex(e => e.UserIdentifier).IsUnique();
            
            // Relationship [User <-1:1-> Diary]
            entity.HasOne<Diary>(e => e.Diary)
                  .WithOne(e => e.User)
                  .HasForeignKey<Diary>(e => e.DiaryUserIdentifier)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("FK_User_Diary");
            
            // Email Address
            entity.HasIndex(e => e.UserEmailAddress).IsUnique();
            entity.Property(e => e.UserEmailAddress).HasMaxLength(50);
            
            // Password
            entity.Property(e => e.UserPasswordHash).HasMaxLength(128);
        });

        // Diary Object
        modelBuilder.Entity<Diary>(entity =>
        {
            // ID
            entity.HasKey(e => e.DiaryIdentifier);
            entity.Property(e => e.DiaryIdentifier).ValueGeneratedOnAdd().HasDefaultValueSql("newsequentialid()");
            entity.HasIndex(e => e.DiaryIdentifier).IsUnique();
        });

        // Log Object
        modelBuilder.Entity<Log>(entity =>
        {
            // ID
            entity.HasKey(e => e.LogIdentifier);
            entity.Property(e => e.LogIdentifier).ValueGeneratedOnAdd().HasDefaultValueSql("newsequentialid()");
            entity.HasIndex(e => e.LogIdentifier).IsUnique();
            
            // Relationship: [Diary <-1:0/M-> Log]
            entity.HasOne(d => d.Diary)
                  .WithMany(p => p.Logs)
                  .HasForeignKey(d => d.LogDiaryIdentifier)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("FK_Log_Diary");
            
            // DateTime
            entity.Property(e => e.LogDateTime).HasDefaultValueSql("getutcdate()");
            entity.Property(e=> e.LogLastUpdated).HasDefaultValueSql("getutcdate()");
            
            // Booleans
            entity.Property(e => e.LogWasCoffeeConsumed).HasDefaultValueSql("0");
            entity.Property(e => e.LogWasMilkConsumed).HasDefaultValueSql("0");
            
            entity.Property(e => e.LogNotes).HasMaxLength(512);
        });
    }
}