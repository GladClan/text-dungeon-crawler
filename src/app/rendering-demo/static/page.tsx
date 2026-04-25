import Link from "next/link";

export const dynamic = "force-static";

const generatedAt = new Date().toISOString();

export default function StaticExamplePage() {
  return (
    <main style={styles.main}>
      <h1 style={styles.h1}>Static Example</h1>
      <p style={styles.p}>This page is pre-rendered and cached as static output.</p>

      <div style={styles.box}>
        <div><strong>Generated at (UTC):</strong> {generatedAt}</div>
        <div><strong>Rendering mode:</strong> force-static</div>
      </div>

      <p style={styles.p}>In production, this timestamp should stay the same across refreshes until a new build/deploy.</p>
      <Link href="/rendering-demo">Back to rendering demo index</Link>
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
  p: {
    margin: 0,
    lineHeight: 1.5,
  },
  box: {
    border: "1px solid #bbb",
    borderRadius: 12,
    padding: "1rem",
    display: "flex",
    flexDirection: "column",
    gap: "0.5rem",
    background: "#f8f8f8",
  },
};
