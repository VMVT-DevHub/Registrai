
-- CREATE EXTENSION pg_trgm; CREATE EXTENSION unaccent;

SET SESSION AUTHORIZATION "_master_admin";

DROP VIEW IF EXISTS jar.v_app_detales;
DROP VIEW IF EXISTS jar.v_app_search_name;
DROP VIEW IF EXISTS jar.v_app_search_id;

DROP MATERIALIZED VIEW IF EXISTS jar.v_app_data;
CREATE MATERIALIZED VIEW jar.v_app_data AS
	SELECT ja_kodas, ja_pavadinimas, adresas, aob_kodas, form_kodas, form_pavadinimas, 
		status_kodas, stat_pavadinimas, stat_data, reg_data, aob_data, isreg_data, formavimo_data, status_kodas<>10 aktyvus,
		REPLACE(REPLACE(.unaccent(lower(ja_pavadinimas)),'"',''),',','') srh1, ja_kodas::varchar(30) srh2,
		60-CAST(TO_CHAR(COALESCE(aob_data,stat_data)+INTERVAL '20 years', 'YYMMDD') AS INTEGER)/10000
		+30+CASE WHEN status_kodas=10 THEN 31 ELSE status_kodas END 
		+CASE WHEN aob_kodas is null THEN 0 ELSE -30 END as sort
	FROM jar.data;


CREATE VIEW jar.v_app_detales AS SELECT ja_kodas, ja_pavadinimas, adresas, aob_kodas, form_kodas, form_pavadinimas, 
	status_kodas, stat_pavadinimas, stat_data, reg_data, aob_data, isreg_data, formavimo_data FROM jar.v_app_data;

CREATE VIEW jar.v_app_search_name AS SELECT ja_kodas "ID", ja_pavadinimas "Pavad", adresas "Adresas", aob_kodas "AobKodas", form_kodas "FormKodas", form_pavadinimas "Forma", 
	status_kodas "StatusKodas", stat_pavadinimas "Statusas", aktyvus "Active", srh1 search, sort FROM jar.v_app_data;

CREATE VIEW jar.v_app_search_id AS SELECT ja_kodas "ID", ja_pavadinimas "Pavad", adresas "Adresas", aob_kodas "AobKodas", form_kodas "FormKodas", form_pavadinimas "Forma", 
	status_kodas "StatusKodas", stat_pavadinimas "Statusas", aktyvus "Active", srh2 search, sort FROM jar.v_app_data;

	
SET SESSION AUTHORIZATION "postgres";
	
CREATE INDEX jar_v_app_data_id_idx on jar.v_app_data (ja_kodas);
CREATE INDEX jar_v_app_data_aob_idx on jar.v_app_data (aob_kodas);
CREATE INDEX jar_v_app_data_form_idx on jar.v_app_data (form_kodas);
CREATE INDEX jar_v_app_data_status_idx on jar.v_app_data (status_kodas);
CREATE INDEX jar_v_app_data_aktyvus_idx on jar.v_app_data (aktyvus);
CREATE INDEX jar_v_app_data_sort_idx on jar.v_app_data (sort);
CREATE INDEX jar_v_app_data_srh2_idx on jar.v_app_data (srh2);

CREATE INDEX jar_v_app_data_search_idx ON jar.v_app_data USING gin(srh1 gin_trgm_ops);



/*

-- pridedamas REFRESH MATERIALIZED VIEW jar.v_app_data;

CREATE OR REPLACE FUNCTION jar.data_load()
	RETURNS TABLE(id bigint, source character varying, "table" character varying, status boolean, items bigint, duration integer, inserted integer, updated integer, deleted integer) LANGUAGE 'plpgsql' AS $BODY$
	DECLARE imp timestamp(3)=timezone('utc'::text, now()); strt timestamp; ins integer; upd integer; del integer; lst varchar(255)[][]; BEGIN
	strt:=clock_timestamp(); SELECT e.ins,e.upd,e.del,clock_timestamp() FROM jar.load_jar() e INTO ins,upd,del;
	INSERT INTO jar.log_import (log_date,log_source,log_table,log_status,log_items,log_duration,log_data_ins,log_data_upd,log_data_del) 
	  VALUES (imp,'load_jar','data',true,ins+upd+del,EXTRACT(milliseconds FROM age(clock_timestamp(),strt)),ins,upd,del); 
	REFRESH MATERIALIZED VIEW jar.v_app_data;
	RETURN QUERY SELECT log_id, log_source, log_table, log_status, log_items, log_duration, log_data_ins,log_data_upd,log_data_del FROM jar.log_import WHERE log_date = imp; END; $BODY$;

*/



















