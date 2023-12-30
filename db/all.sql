create table public.users
(
    username     varchar                                                   not null
        constraint users_pk
            primary key,
    password     varchar(64)                                               not null,
    role         varchar                                                   not null,
    coins        integer             default 0                             not null,
    display_name varchar                                                   not null,
    bio          varchar,
    image        varchar,
    deck         character varying[] default ARRAY []::character varying[] not null
);

alter table public.users
    owner to postgres;

create table public.cards
(
    id     varchar(64)           not null
        constraint cards_pk
            primary key,
    name   varchar               not null,
    damage real                  not null,
    owner  varchar,
    locked boolean default false not null
);

alter table public.cards
    owner to postgres;

create table public.packages
(
    id       varchar(64)   not null
        constraint packages_pk
            primary key,
    card_ids varchar(64)[] not null
);

alter table public.packages
    owner to postgres;

create table public.stats
(
    username varchar not null
        constraint stats_pk
            primary key
        constraint stats_users_username_fk
            references public.users,
    elo      integer not null,
    wins     integer not null,
    losses   integer not null
);

alter table public.stats
    owner to postgres;

create table public.trade_offers
(
    id             varchar not null
        constraint trade_offers_pk
            primary key,
    owner          varchar not null
        constraint trade_offers_users_username_fk
            references public.users,
    card_to_trade  varchar not null
        constraint trade_offers_cards_id_fk
            references public.cards,
    card_type      varchar not null,
    minimum_damage integer not null
);

alter table public.trade_offers
    owner to postgres;

