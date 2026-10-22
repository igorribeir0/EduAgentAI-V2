
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],

  server: {
    host: '127.0.0.1',
    port: 5173,
    strictPort: true,

    allowedHosts: [
      'gdp-albany-dimensional-arabic.trycloudflare.com',
    ],

    proxy: {
      '/api': {
        target: 'http://localhost:5186',
        changeOrigin: true,
      },
    },
  },
})
