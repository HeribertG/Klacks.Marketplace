/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './Pages/**/*.{razor,cshtml}',
    './Shared/**/*.razor',
    './App.razor',
    './_Imports.razor'
  ],
  darkMode: 'class',
  theme: {
    extend: {
      colors: {
        'background': '#fcf8fa',
        'surface': '#fcf8fa',
        'surface-bright': '#fcf8fa',
        'surface-dim': '#dcd9db',
        'surface-container': '#f0edef',
        'surface-container-high': '#eae7e9',
        'surface-container-highest': '#e4e2e3',
        'surface-container-low': '#f6f3f4',
        'surface-container-lowest': '#ffffff',
        'surface-variant': '#e4e2e3',
        'surface-tint': '#565e71',
        'primary': '#000000',
        'primary-container': '#141b2c',
        'primary-fixed': '#dbe2f9',
        'primary-fixed-dim': '#bfc6dc',
        'secondary': '#0050cc',
        'secondary-container': '#0266ff',
        'secondary-fixed': '#dae1ff',
        'secondary-fixed-dim': '#b3c5ff',
        'tertiary': '#000000',
        'tertiary-container': '#002114',
        'tertiary-fixed': '#85f8c4',
        'tertiary-fixed-dim': '#68dba9',
        'on-background': '#1b1b1d',
        'on-surface': '#1b1b1d',
        'on-surface-variant': '#45474c',
        'on-primary': '#ffffff',
        'on-primary-container': '#7c8498',
        'on-primary-fixed': '#141b2c',
        'on-primary-fixed-variant': '#3f4759',
        'on-secondary': '#ffffff',
        'on-secondary-container': '#f9f7ff',
        'on-secondary-fixed': '#001849',
        'on-secondary-fixed-variant': '#003fa4',
        'on-tertiary': '#ffffff',
        'on-tertiary-container': '#069669',
        'on-tertiary-fixed': '#002114',
        'on-tertiary-fixed-variant': '#005137',
        'outline': '#76777d',
        'outline-variant': '#c6c6cd',
        'error': '#ba1a1a',
        'error-container': '#ffdad6',
        'on-error': '#ffffff',
        'on-error-container': '#93000a',
        'inverse-surface': '#303032',
        'inverse-on-surface': '#f3f0f1',
        'inverse-primary': '#bfc6dc'
      },
      fontFamily: {
        'headline': ['Inter', 'sans-serif'],
        'body': ['Inter', 'sans-serif'],
        'label': ['Inter', 'sans-serif'],
        'sans': ['Inter', 'system-ui', '-apple-system', 'BlinkMacSystemFont', 'Segoe UI', 'Roboto', 'sans-serif']
      },
      borderRadius: {
        'DEFAULT': '0.25rem',
        'lg': '0.5rem',
        'xl': '0.75rem',
        'full': '9999px'
      }
    }
  },
  plugins: []
};
