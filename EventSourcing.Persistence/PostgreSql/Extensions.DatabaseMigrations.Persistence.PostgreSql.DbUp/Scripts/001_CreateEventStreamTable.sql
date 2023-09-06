CREATE TABLE IF NOT EXISTS EventStream (
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
    PRIMARY KEY (StreamId, EntrySequence)
);

CREATE INDEX IDX_StreamId ON EventStream (StreamId);
CREATE INDEX IDX_StreamId_EntrySequence ON EventStream (StreamId, EntrySequence ASC);