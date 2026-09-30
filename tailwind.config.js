/**
 * Tailwind configuration for the Klacks Store.
 * The palette is the Klacks brand theme ("Petrol + Amber"), mirrored from
 * Klacks.Ui/src/assets/standard-styles/colors.scss (:root[data-theme="klacks"]).
 * Token names keep the Material-style roles the Razor markup already uses; only the values are Klacks.
 * Rule: petrol for everything interactive, amber (highlight) only as a surface with ink text, never as text colour.
 */
const klacks = {
  petrol: '#0e6e6b',
  petrolDark: '#0b4f4d',
  petrolText: '#0b5f5c',
  petrolSoft: '#d5ecea',
  petrolMuted: '#5fa8a5',
  amber: '#f2a516',
  amberSoft: '#fdf0d2',
  amberText: '#5c3d00',
  ink: '#16181d',
  label: '#464e5f',
  inactive: '#777777',
  white: '#ffffff',
  window: '#f4f6f5',
  rowHover: '#eef4f3',
  subCard: '#e1e9e8',
  ownHeadlineHover: '#d3dedc',
  gridContainer: '#2f3d3c',
  cardBorder: '#cfd8dc',
  successBackground: '#d4edda',
  successText: '#155724',
  dangerBackground: '#f8d7da',
  dangerText: '#721c24',
  danger: '#cc0000',
  neutralBackground: '#e2e3e5',
  neutralText: '#383d41',
  externBackground: '#dbeafe',
  externText: '#1d4ed8',
  customerText: '#8a5a00',
  purpleBackground: '#ece7f7',
  purpleText: '#5e45a0'
};

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
        'background': klacks.window,
        'surface': klacks.window,
        'surface-bright': klacks.window,
        'surface-dim': klacks.ownHeadlineHover,
        'surface-container': klacks.subCard,
        'surface-container-high': klacks.ownHeadlineHover,
        'surface-container-highest': klacks.ownHeadlineHover,
        'surface-container-low': klacks.rowHover,
        'surface-container-lowest': klacks.white,
        'surface-variant': klacks.subCard,
        'surface-tint': klacks.petrol,
        'primary': klacks.ink,
        'primary-container': klacks.petrolDark,
        'primary-fixed': klacks.petrolSoft,
        'primary-fixed-dim': klacks.petrolMuted,
        'secondary': klacks.petrol,
        'secondary-container': klacks.petrolDark,
        'secondary-fixed': klacks.petrolSoft,
        'secondary-fixed-dim': klacks.petrolMuted,
        'tertiary': klacks.successText,
        'tertiary-container': klacks.petrolDark,
        'tertiary-fixed': klacks.successBackground,
        'tertiary-fixed-dim': klacks.petrolMuted,
        'highlight': klacks.amber,
        'highlight-container': klacks.amberSoft,
        'on-background': klacks.ink,
        'on-surface': klacks.ink,
        'on-surface-variant': klacks.label,
        'on-primary': klacks.white,
        'on-primary-container': klacks.petrolSoft,
        'on-primary-fixed': klacks.petrolDark,
        'on-primary-fixed-variant': klacks.petrolText,
        'on-secondary': klacks.white,
        'on-secondary-container': klacks.white,
        'on-secondary-fixed': klacks.petrolDark,
        'on-secondary-fixed-variant': klacks.petrolText,
        'on-tertiary': klacks.white,
        'on-tertiary-container': klacks.petrol,
        'on-tertiary-fixed': klacks.successText,
        'on-tertiary-fixed-variant': klacks.successText,
        'on-highlight': klacks.ink,
        'on-highlight-container': klacks.amberText,
        'outline': klacks.inactive,
        'outline-variant': klacks.cardBorder,
        'error': klacks.danger,
        'error-container': klacks.dangerBackground,
        'on-error': klacks.white,
        'on-error-container': klacks.dangerText,
        'inverse-surface': klacks.ink,
        'inverse-on-surface': klacks.subCard,
        'inverse-primary': klacks.petrolMuted,
        'category-communication': klacks.externBackground,
        'on-category-communication': klacks.externText,
        'category-erp': klacks.petrolSoft,
        'on-category-erp': klacks.petrolText,
        'category-accounting': klacks.amberSoft,
        'on-category-accounting': klacks.customerText,
        'category-reporting': klacks.purpleBackground,
        'on-category-reporting': klacks.purpleText,
        'category-integration': klacks.subCard,
        'on-category-integration': klacks.gridContainer,
        'category-default': klacks.neutralBackground,
        'on-category-default': klacks.neutralText
      },
      fontFamily: {
        'headline': ['Roboto', 'sans-serif'],
        'body': ['Roboto', 'sans-serif'],
        'label': ['Roboto', 'sans-serif'],
        'sans': ['Roboto', 'system-ui', '-apple-system', 'BlinkMacSystemFont', 'Segoe UI', 'sans-serif']
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
