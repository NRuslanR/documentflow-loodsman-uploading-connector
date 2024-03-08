using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Npgsql;
using Npgsql.EntityFrameworkCore;
using Npgsql.Util;

namespace UMP.DocumentFlow.LoodsmanUploadingConnector.Uploading.Management.EntityFramework
{
    public class DbLoodsmanDocumentUploadingInfoContext : DbContext
    {
        private readonly ILoodsmanDocumentUploadingInfoTableFieldNames fieldNames;

        public DbLoodsmanDocumentUploadingInfoContext(DbContextOptions<DbLoodsmanDocumentUploadingInfoContext> options,
            ILoodsmanDocumentUploadingInfoTableFieldNames fieldNames) : base(options)
        {
            this.fieldNames = fieldNames; 
        }

        public DbLoodsmanDocumentUploadingInfoContext(ILoodsmanDocumentUploadingInfoTableFieldNames fieldNames)
        {
            this.fieldNames = fieldNames;
        }

        public DbSet<LoodsmanDocumentUploadingRecordInfo> UploadingRecordInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LoodsmanDocumentUploadingRecordInfo>(entity =>
            {
                entity
                    .Property(e => e.UploadingStatus)
                    .HasColumnName(fieldNames.UploadingStatusFieldName)
                    .HasConversion(new EnumToStringConverter<LoodsmanDocumentUploadingStatus>());

                entity
                    .Property(e => e.UploadingRequestedDateTime)
                    .HasColumnName(fieldNames.UploadingRequestedDateTimeFieldName);

                entity
                    .Property(e => e.UploadingDateTime)
                    .HasColumnName(fieldNames.UploadingDateTimeFieldName);

                entity
                    .Property(e => e.CancelingDateTime)
                    .HasColumnName(fieldNames.CancelingDateTimeFieldName);

                entity
                    .Property(e => e.ErrorMessage)
                    .HasColumnName(fieldNames.ErrorMessageFieldName);

                entity
                    .Property(e => e.DocumentJson)
                    .HasColumnName(fieldNames.DocumentJsonFieldName);

                entity
                    .Property(e => e.CancelationRequestedDateTime)
                    .HasColumnName(fieldNames.CancelationRequestedDateTimeFieldName);

                entity
                    .Property(e => e.CanceledDateTime)
                    .HasColumnName(fieldNames.CanceledDateTimeFieldName);

                entity
                    .Property(e => e.UploadedDateTime)
                    .HasColumnName(fieldNames.UploadedDateTimeFieldName);

                entity
                    .Property(e => e.CancelerId)
                    .HasColumnName(fieldNames.CancelerIdFieldName);

                entity
                    .Property(e => e.DocumentId)
                    .HasColumnName(fieldNames.DocumentIdFieldName);

                entity
                    .Property(e => e.Id)
                    .HasColumnName(fieldNames.IdFieldName);

                entity
                    .Property(e => e.InitiatorId)
                    .HasColumnName(fieldNames.InitiatorIdFieldName);
            });
        }
    }
}