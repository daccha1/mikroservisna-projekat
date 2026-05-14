import { useState, useEffect, useCallback } from "react";

function joinUrl(baseUrl, ...segments) {
  const cleanedBase = baseUrl.replace(/\/+$/, "");
  const cleanedSegments = segments
    .filter(Boolean)
    .map((segment) => String(segment).replace(/^\/+|\/+$/g, ""));

  return [cleanedBase, ...cleanedSegments].join("/");
}

function camelToLabel(str) {
  return str.replace(/([A-Z])/g, " $1").replace(/^./, (s) => s.toUpperCase());
}

function ValueCell({ value }) {
  if (Array.isArray(value)) {
    if (value.length === 0) {
      return <span className="text-stone-600 italic">-</span>;
    }

    return (
      <div className="flex flex-wrap gap-1">
        {value.map((v, i) => (
          <span
            key={i}
            className="bg-amber-500/10 text-amber-300 text-xs px-2 py-0.5 rounded-full border border-amber-500/20"
          >
            {v}
          </span>
        ))}
      </div>
    );
  }

  if (value === null || value === undefined) {
    return <span className="text-stone-600 italic">-</span>;
  }

  if (typeof value === "boolean") return <span>{value ? "Yes" : "No"}</span>;

  if (typeof value === "string" && value.includes("T")) {
    const d = new Date(value);
    if (!Number.isNaN(d.getTime())) return <span>{d.toLocaleString()}</span>;
  }

  return <span>{String(value)}</span>;
}

function Modal({ title, onClose, children }) {
  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/70 backdrop-blur-sm px-4">
      <div className="bg-stone-900 border border-stone-700 rounded-2xl w-full max-w-lg shadow-2xl">
        <div className="flex items-center justify-between px-6 py-4 border-b border-stone-800">
          <h3
            className="font-bold text-stone-100 text-lg"
            style={{ fontFamily: "'Playfair Display', serif" }}
          >
            {title}
          </h3>
          <button
            onClick={onClose}
            className="text-stone-500 hover:text-stone-200 text-2xl leading-none"
          >
            &times;
          </button>
        </div>
        <div className="px-6 py-5">{children}</div>
      </div>
    </div>
  );
}

function FormFields({ fields, fieldLabels, fieldTypes, values, onChange }) {
  return (
    <div className="space-y-3">
      {fields.map((field) => (
        <div key={field}>
          <label className="block text-stone-400 text-xs mb-1 tracking-wide uppercase">
            {fieldLabels[field] || camelToLabel(field)}
          </label>
          <input
            type={fieldTypes[field] || "text"}
            value={values[field] ?? ""}
            onChange={(e) => onChange(field, e.target.value)}
            className="w-full bg-stone-800 border border-stone-700 rounded-lg px-3 py-2 text-stone-100 text-sm focus:outline-none focus:border-amber-500 transition-colors"
          />
        </div>
      ))}
    </div>
  );
}

export default function EntityPage({ entity, onBack }) {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const [searchId, setSearchId] = useState("");
  const [searchResult, setSearchResult] = useState(null);
  const [searchError, setSearchError] = useState("");

  const [showPost, setShowPost] = useState(false);
  const [showUpdate, setShowUpdate] = useState(false);
  const [updateId, setUpdateId] = useState("");
  const [formValues, setFormValues] = useState({});

  const fetchAll = useCallback(async () => {
    setLoading(true);
    setError("");

    try {
      const res = await fetch(joinUrl(entity.api.baseUrl, entity.api.path));
      if (!res.ok) throw new Error(`HTTP ${res.status}`);
      const json = await res.json();
      setData(Array.isArray(json) ? json : []);
    } catch (e) {
      setError("Failed to fetch: " + e.message);
    } finally {
      setLoading(false);
    }
  }, [entity.api.baseUrl, entity.api.path]);

  useEffect(() => {
    fetchAll();
    setSearchResult(null);
    setSearchId("");
    setSearchError("");
    setFormValues({});
    setSuccess("");
    setError("");
  }, [fetchAll]);

  const handleSearch = async () => {
    if (!searchId) return;

    setSearchError("");
    setSearchResult(null);

    try {
      const res = await fetch(joinUrl(entity.api.baseUrl, entity.api.path, searchId));
      if (!res.ok) throw new Error(`Not found (HTTP ${res.status})`);
      const json = await res.json();
      setSearchResult(json);
    } catch (e) {
      setSearchError(e.message);
    }
  };

  const handlePost = async () => {
    setSuccess("");
    setError("");

    try {
      const body = { ...formValues };
      entity.fields.request.forEach((f) => {
        if (entity.fieldTypes[f] === "number") body[f] = Number(body[f]);
      });

      const res = await fetch(joinUrl(entity.api.baseUrl, entity.api.path, "add"), {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(body),
      });

      if (!res.ok) {
        const txt = await res.text();
        throw new Error(txt || `HTTP ${res.status}`);
      }

      setSuccess("Record added successfully.");
      setShowPost(false);
      setFormValues({});
      fetchAll();
    } catch (e) {
      setError("POST failed: " + e.message);
      setShowPost(false);
    }
  };

  const handleUpdate = async () => {
    setSuccess("");
    setError("");

    try {
      const body = { ...formValues };
      entity.fields.request.forEach((f) => {
        if (entity.fieldTypes[f] === "number") body[f] = Number(body[f]);
      });

      const res = await fetch(joinUrl(entity.api.baseUrl, entity.api.path, "update", updateId), {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(body),
      });

      if (!res.ok) {
        const txt = await res.text();
        throw new Error(txt || `HTTP ${res.status}`);
      }

      setSuccess(`Record #${updateId} updated successfully.`);
      setShowUpdate(false);
      setFormValues({});
      setUpdateId("");
      fetchAll();
    } catch (e) {
      setError("PUT failed: " + e.message);
      setShowUpdate(false);
    }
  };

  const responseFields = entity.fields.response;

  return (
    <main className="pt-14 min-h-screen">
      <div className="max-w-7xl mx-auto px-6 py-10">
        <div className="flex items-center gap-4 mb-8">
          <button
            onClick={onBack}
            className="text-stone-500 hover:text-amber-400 transition-colors text-sm flex items-center gap-1"
          >
            &larr; Back
          </button>
          <span className="text-stone-700">/</span>
          <h2
            className="text-3xl font-bold text-stone-100"
            style={{ fontFamily: "'Playfair Display', serif" }}
          >
            <span className="mr-3">{entity.icon}</span>
            {entity.label}
          </h2>
        </div>

        <p className="text-stone-500 text-xs tracking-widest uppercase mb-6">
          API: {entity.api.baseUrl}/{entity.api.path}
        </p>

        <div className="flex flex-wrap gap-3 mb-6">
          <div className="flex gap-2">
            <input
              type="number"
              placeholder="Search by ID..."
              value={searchId}
              onChange={(e) => setSearchId(e.target.value)}
              onKeyDown={(e) => e.key === "Enter" && handleSearch()}
              className="bg-stone-800 border border-stone-700 rounded-lg px-3 py-2 text-stone-100 text-sm w-44 focus:outline-none focus:border-amber-500 transition-colors"
            />
            <button
              onClick={handleSearch}
              className="bg-stone-800 hover:bg-stone-700 border border-stone-700 text-stone-200 text-sm px-4 py-2 rounded-lg transition-colors"
            >
              Get by ID
            </button>
          </div>

          <div className="flex-1" />

          <button
            onClick={() => {
              setFormValues({});
              setShowPost(true);
            }}
            className="bg-amber-500 hover:bg-amber-400 text-stone-950 font-semibold text-sm px-4 py-2 rounded-lg transition-colors"
          >
            + Add New
          </button>
          <button
            onClick={() => {
              setFormValues({});
              setShowUpdate(true);
            }}
            className="bg-stone-700 hover:bg-stone-600 text-stone-100 text-sm px-4 py-2 rounded-lg transition-colors border border-stone-600"
          >
            Update by ID
          </button>
          <button
            onClick={fetchAll}
            className="bg-stone-800 hover:bg-stone-700 border border-stone-700 text-stone-300 text-sm px-4 py-2 rounded-lg transition-colors"
          >
            Refresh
          </button>
        </div>

        {success && (
          <div className="mb-4 px-4 py-3 rounded-lg bg-emerald-500/10 border border-emerald-500/30 text-emerald-400 text-sm">
            {success}
          </div>
        )}
        {error && (
          <div className="mb-4 px-4 py-3 rounded-lg bg-red-500/10 border border-red-500/30 text-red-400 text-sm">
            {error}
          </div>
        )}

        {searchResult && (
          <div className="mb-6 p-4 bg-amber-500/5 border border-amber-500/20 rounded-xl">
            <div className="flex items-center justify-between mb-3">
              <span className="text-amber-400 text-xs tracking-widest uppercase font-medium">
                Search Result - ID {searchId}
              </span>
              <button
                onClick={() => setSearchResult(null)}
                className="text-stone-500 hover:text-stone-300 text-sm"
              >
                Clear
              </button>
            </div>
            <div className="grid grid-cols-2 md:grid-cols-3 gap-3">
              {responseFields.map((f) => (
                <div key={f}>
                  <p className="text-stone-500 text-xs mb-0.5">
                    {entity.fieldLabels[f] || camelToLabel(f)}
                  </p>
                  <div className="text-stone-200 text-sm">
                    <ValueCell value={searchResult[f]} />
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}
        {searchError && (
          <div className="mb-4 px-4 py-3 rounded-lg bg-red-500/10 border border-red-500/30 text-red-400 text-sm">
            Search: {searchError}
          </div>
        )}

        {loading ? (
          <div className="text-stone-500 py-16 text-center">Loading...</div>
        ) : data.length === 0 ? (
          <div className="text-stone-500 py-16 text-center border border-stone-800 rounded-2xl">
            No records found.
          </div>
        ) : (
          <div className="overflow-x-auto rounded-2xl border border-stone-800">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-stone-800 bg-stone-900/80">
                  {responseFields.map((f) => (
                    <th
                      key={f}
                      className="text-left px-4 py-3 text-stone-400 text-xs tracking-widest uppercase font-medium whitespace-nowrap"
                    >
                      {entity.fieldLabels[f] || camelToLabel(f)}
                    </th>
                  ))}
                </tr>
              </thead>
              <tbody>
                {data.map((row, i) => (
                  <tr
                    key={row.id ?? i}
                    className="border-b border-stone-800/50 hover:bg-stone-800/30 transition-colors"
                  >
                    {responseFields.map((f) => (
                      <td key={f} className="px-4 py-3 text-stone-300 max-w-xs">
                        <ValueCell value={row[f]} />
                      </td>
                    ))}
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

        <p className="text-stone-600 text-xs mt-3">{data.length} record(s) loaded</p>
      </div>

      {showPost && (
        <Modal title={`Add New ${entity.label}`} onClose={() => setShowPost(false)}>
          <FormFields
            fields={entity.fields.request}
            fieldLabels={entity.fieldLabels}
            fieldTypes={entity.fieldTypes}
            values={formValues}
            onChange={(f, v) => setFormValues((prev) => ({ ...prev, [f]: v }))}
          />
          <div className="flex gap-3 mt-5">
            <button
              onClick={handlePost}
              className="flex-1 bg-amber-500 hover:bg-amber-400 text-stone-950 font-semibold py-2 rounded-lg transition-colors text-sm"
            >
              Submit
            </button>
            <button
              onClick={() => setShowPost(false)}
              className="flex-1 bg-stone-800 hover:bg-stone-700 text-stone-300 py-2 rounded-lg transition-colors text-sm"
            >
              Cancel
            </button>
          </div>
        </Modal>
      )}

      {showUpdate && (
        <Modal title={`Update ${entity.label}`} onClose={() => setShowUpdate(false)}>
          <div className="mb-4">
            <label className="block text-stone-400 text-xs mb-1 tracking-wide uppercase">
              Record ID to update
            </label>
            <input
              type="number"
              value={updateId}
              onChange={(e) => setUpdateId(e.target.value)}
              className="w-full bg-stone-800 border border-stone-700 rounded-lg px-3 py-2 text-stone-100 text-sm focus:outline-none focus:border-amber-500 transition-colors"
              placeholder="Enter ID..."
            />
          </div>
          <div className="border-t border-stone-800 pt-4">
            <FormFields
              fields={entity.fields.request}
              fieldLabels={entity.fieldLabels}
              fieldTypes={entity.fieldTypes}
              values={formValues}
              onChange={(f, v) => setFormValues((prev) => ({ ...prev, [f]: v }))}
            />
          </div>
          <div className="flex gap-3 mt-5">
            <button
              onClick={handleUpdate}
              className="flex-1 bg-amber-500 hover:bg-amber-400 text-stone-950 font-semibold py-2 rounded-lg transition-colors text-sm"
            >
              Update
            </button>
            <button
              onClick={() => setShowUpdate(false)}
              className="flex-1 bg-stone-800 hover:bg-stone-700 text-stone-300 py-2 rounded-lg transition-colors text-sm"
            >
              Cancel
            </button>
          </div>
        </Modal>
      )}
    </main>
  );
}

