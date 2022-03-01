CREATE TABLE EventStreamStaging (
    StagingId                           UNIQUEIDENTIFIER        NOT NULL,
    StreamId                            UNIQUEIDENTIFIER        NOT NULL,
    EntrySequence                       BIGINT                  NOT NULL,
    EntryId                             UNIQUEIDENTIFIER        NOT NULL,
    EventContent                        VARCHAR(MAX)            NOT NULL,
    EventContentSerializationFormat     VARCHAR(MAX)            NOT NULL,
    EventTypeIdentifier                 VARCHAR(MAX)            NOT NULL,
    EventTypeIdentifierFormat           VARCHAR(MAX)            NOT NULL,
    CausationId                         UNIQUEIDENTIFIER        NOT NULL,
    CreationTime                        DATETIMEOFFSET          NOT NULL,
    CorrelationId                       UNIQUEIDENTIFIER        NOT NULL,
    PRIMARY KEY (StagingId, StreamId, EntrySequence)
);

CREATE INDEX IDX_StagingId ON EventStreamStaging (StagingId);