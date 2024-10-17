
-- CREATE EXTENSION pg_trgm; CREATE EXTENSION unaccent SCHEMA jar;

SET SESSION AUTHORIZATION "_master_admin";

DROP VIEW IF EXISTS jar.v_app_detales;
DROP VIEW IF EXISTS jar.v_app_search;

DROP MATERIALIZED VIEW IF EXISTS jar.v_app_data;
CREATE MATERIALIZED VIEW jar.v_app_data AS
	SELECT ja_kodas, ja_pavadinimas, adresas, aob_kodas, form_kodas, form_pavadinimas, 
		status_kodas, stat_pavadinimas, stat_data, reg_data, aob_data, isreg_data, formavimo_data, status_kodas<>10 aktyvus,
		ja_kodas || ' ' || REPLACE(REPLACE(ar.unaccent(lower(ja_pavadinimas)),'"',''),',','') search,	
		60-CAST(TO_CHAR(COALESCE(aob_data,stat_data)+INTERVAL '20 years', 'YYMMDD') AS INTEGER)/10000
		+30+CASE WHEN status_kodas=10 THEN 31 ELSE status_kodas END 
		+CASE WHEN aob_kodas is null THEN 0 ELSE -30 END as sort
	FROM jar.data;

CREATE INDEX jar_v_app_data_id_idx on jar.v_app_data (ja_kodas);
CREATE INDEX jar_v_app_data_aob_idx on jar.v_app_data (aob_kodas);
CREATE INDEX jar_v_app_data_form_idx on jar.v_app_data (form_kodas);
CREATE INDEX jar_v_app_data_status_idx on jar.v_app_data (status_kodas);
CREATE INDEX jar_v_app_data_aktyvus_idx on jar.v_app_data (aktyvus);
CREATE INDEX jar_v_app_data_sort_idx on jar.v_app_data (sort);

CREATE INDEX jar_v_app_data_search_idx ON jar.v_app_data USING gin(search gin_trgm_ops);


CREATE VIEW jar.v_app_detales AS SELECT ja_kodas, ja_pavadinimas, adresas, aob_kodas, form_kodas, form_pavadinimas, 
	status_kodas, stat_pavadinimas, stat_data, reg_data, aob_data, isreg_data, formavimo_data FROM jar.v_app_data;

CREATE VIEW jar.v_app_search AS SELECT ja_kodas "ID", ja_pavadinimas "Pavad", adresas "Adresas", aob_kodas "AobKodas", form_kodas "FormKodas", form_pavadinimas "Forma", 
	status_kodas "StatusKodas", stat_pavadinimas "Statusas", aktyvus "Active", search, sort FROM jar.v_app_data;































