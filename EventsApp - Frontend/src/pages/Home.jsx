export default function Home({ entities, onSelect }) {
  return (
    <main className="pt-14">
      {/* Hero */}
      <section className="relative flex flex-col items-center justify-center text-center px-6 py-32 overflow-hidden">
        {/* Background grid */}
        <div className="absolute inset-0 opacity-5"
          style={{
            backgroundImage: "linear-gradient(#d6d3d1 1px, transparent 1px), linear-gradient(90deg, #d6d3d1 1px, transparent 1px)",
            backgroundSize: "48px 48px",
          }}
        />
        {/* Glow */}
        <div className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[600px] h-[300px] bg-amber-500/10 rounded-full blur-[100px] pointer-events-none" />

        <p className="text-amber-400 text-xs tracking-[0.3em] uppercase mb-6 font-medium">Admin Dashboard</p>
        <h1
          className="text-6xl md:text-8xl font-bold text-stone-100 leading-none mb-6"
          style={{ fontFamily: "'Playfair Display', serif" }}
        >
          Event
          <br />
          <span className="text-amber-400">Management</span>
        </h1>
        <p className="text-stone-400 text-lg max-w-md">
          Select an entity below to browse, search, add, or update records.
        </p>
      </section>

      {/* Entity Cards */}
      <section className="max-w-7xl mx-auto px-6 pb-24">
        <p className="text-stone-500 text-xs tracking-widest uppercase mb-8">Choose entity</p>
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
          {entities.map((entity) => (
            <button
              key={entity.key}
              onClick={() => onSelect(entity)}
              className="group relative text-left p-6 rounded-2xl border border-stone-800 bg-stone-900/50 hover:bg-stone-800/80 hover:border-amber-500/50 transition-all duration-200 overflow-hidden"
            >
              {/* Hover glow */}
              <div className="absolute inset-0 bg-gradient-to-br from-amber-500/0 to-amber-500/0 group-hover:from-amber-500/5 group-hover:to-transparent transition-all duration-300 rounded-2xl" />
              <div className="flex items-start justify-between mb-4">
                <span className="text-3xl">{entity.icon}</span>
                <span className="text-stone-600 group-hover:text-amber-400 transition-colors text-xl">→</span>
              </div>
              <h3
                className="text-xl font-bold text-stone-100 mb-1 group-hover:text-amber-300 transition-colors"
                style={{ fontFamily: "'Playfair Display', serif" }}
              >
                {entity.label}
              </h3>
              <p className="text-stone-500 text-sm">
                GET · POST · PUT
              </p>
            </button>
          ))}
        </div>
      </section>
    </main>
  );
}
