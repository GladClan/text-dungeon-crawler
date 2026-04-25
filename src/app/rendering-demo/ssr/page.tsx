import Link from "next/link";
import { headers } from "next/headers";

export const dynamic = "force-dynamic";

export default async function SsrExamplePage() {
  const requestHeaders = await headers();
  const userAgent = requestHeaders.get("user-agent") ?? "unknown";
  const renderedAt = new Date().toISOString();

  return (
    <main style={styles.main}>
      <h1 style={styles.h1}>SSR Example</h1>
      <p style={styles.p}>This page is rendered on every request.</p>

      <div style={styles.box}>
        <div><strong>Rendered at (UTC):</strong> {renderedAt}</div>
        <div><strong>Rendering mode:</strong> force-dynamic (SSR)</div>
        <div><strong>User-Agent (from request headers):</strong> {userAgent}</div>
      </div>

      <p style={styles.p}>Refresh this page a few times in production. The timestamp should change each request.</p>
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
