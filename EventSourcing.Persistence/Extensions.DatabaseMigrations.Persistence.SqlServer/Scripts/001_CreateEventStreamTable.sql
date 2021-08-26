CREATE TABLE EventStream (
    StreamId                UNIQUEIDENTIFIER        NOT NULL,
    EntrySequence           BIGINT                  NOT NULL,
    EntryId                 UNIQUEIDENTIFIER        NOT NULL,
    EventContent            VARCHAR(MAX)            NOT NULL,
    EventTypeIdentifier     VARCHAR(MAX)            NOT NULL,
    CausationId             UNIQUEIDENTIFIER        NOT NULL,
    CreationTime            DATETIMEOFFSET          NOT NULL,
    CorrelationId           UNIQUEIDENTIFIER        NOT NULL,
    PRIMARY KEY (StreamId, EntrySequence)        
);

CREATE INDEX IDX_StreamId ON EventStream (StreamId);