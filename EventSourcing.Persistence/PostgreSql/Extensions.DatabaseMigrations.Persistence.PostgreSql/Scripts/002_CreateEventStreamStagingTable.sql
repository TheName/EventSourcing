CREATE TABLE EventStreamStaging (
    StagingId                           UUID                    NOT NULL,
    StagingTime                         TIMESTAMP               NOT NULL,
    StagingTimeNanoSeconds              BIGINT                  NOT NULL,
    StreamId                            UUID                    NOT NULL,
    EntrySequence                       BIGINT                  NOT NULL,
    EntryId                             UUID                    NOT NULL,
    EventContent                        VARCHAR                 NOT NULL,
    EventContentSerializationFormat     VARCHAR                 NOT NULL,
    EventTypeIdentifier                 VARCHAR                 NOT NULL,
    EventTypeIdentifierFormat           VARCHAR                 NOT NULL,
    CausationId                         UUID                    NOT NULL,
    CreationTime                        TIMESTAMP               NOT NULL,
    CreationTimeNanoSeconds             BIGINT                  NOT NULL,
    CorrelationId                       UUID                    NOT NULL,
    PRIMARY KEY (StagingId, StreamId, EntrySequence)
);

CREATE INDEX IDX_StagingId ON EventStreamStaging (StagingId);