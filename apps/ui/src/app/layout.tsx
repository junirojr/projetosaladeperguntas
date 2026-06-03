import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "Roquet.q",
  description: "Salas colaborativas de perguntas e respostas",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="pt-BR">
      <body>{children}</body>
    </html>
  );
}
