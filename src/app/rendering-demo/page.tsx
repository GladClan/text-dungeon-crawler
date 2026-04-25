import Link from "next/link";

export default function RenderingDemoIndexPage() {
  return (
    <main style={styles.main}>
      <h1 style={styles.h1}>Rendering Demo</h1>
      <p style={styles.p}>
        These two routes are intentionally simple so you can compare how server-side rendering and static rendering behave.
      </p>

      <div style={styles.cardWrap}>
        <div style={styles.card}>
          <h2 style={styles.h2}>SSR Example</h2>
          <p style={styles.p}>Re-renders on every request. Refreshing should produce a new timestamp in production.</p>
          <Link href="/rendering-demo/ssr">Open SSR example</Link>
        </div>

        <div style={styles.card}>
          <h2 style={styles.h2}>Static Example</h2>
          <p style={styles.p}>Pre-rendered and cached. Timestamp is generated at build time in production.</p>
          <Link href="/rendering-demo/static">Open static example</Link>
        </div>
      </div>
    </main>
  );
}

const styles: { [key: string]: React.CSSProperties } = {
  main: {
    minHeight: "100vh",
    padding: "2rem",
    display: "flex",
    flexDirection: "column",
    gap: "1rem",
  },
  h1: {
    margin: 0,
  },
  h2: {
    marginTop: 0,
    marginBottom: "0.5rem",
  },
  p: {
    margin: 0,
    lineHeight: 1.5,
  },
  cardWrap: {
    display: "grid",
    gridTemplateColumns: "repeat(auto-fit, minmax(260px, 1fr))",
    gap: "1rem",
    marginTop: "0.5rem",
  },
  card: {
    border: "1px solid #bbb",
    borderRadius: 12,
    padding: "1rem",
    display: "flex",
    flexDirection: "column",
    gap: "0.75rem",
    background: "#f8f8f8",
  },
};
