/**
 * Tailwind configuration for the Klacks Store.
 * The palette is the Klacks brand theme ("Petrol + Amber"), mirrored from
 * Klacks.Ui/src/assets/standard-styles/colors.scss (:root[data-theme="klacks"]).
 * Token names keep the Material-style roles the Razor markup already uses; only the values are Klacks.
 * Rule: petrol for everything interactive, amber (highlight) only as a surface with ink text, never as text colour.
 * Dark mode: lightColors/darkColors are emitted as CSS variables (:root / :root.dark); theme-mode.js sets the class.
 */
const plugin = require('tailwindcss/plugin');

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

// Light palette: the token roles above, unchanged. "secondary-fill" is the petrol behind white text (bg-secondary),
// "link-hover" the hover colour of navigation links.
const lightColors = {
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
  'on-category-default': klacks.neutralText,
  'secondary-fill': klacks.petrol,
  'link-hover': klacks.petrolDark
};

// Dark palette, active while <html> carries the "dark" class (wwwroot/js/theme-mode.js). Same keys as lightColors.
// Hero (primary-container), amber (highlight/on-highlight) and the footer (inverse-surface) stay dark in both modes.
const darkColors = {
  'background': '#111414',
  'surface': '#111414',
  'surface-bright': '#111414',
  'surface-dim': '#2b3232',
  'surface-container': '#222828',
  'surface-container-high': '#2b3232',
  'surface-container-highest': '#2b3232',
  'surface-container-low': '#171c1c',
  'surface-container-lowest': '#1c2121',
  'surface-variant': '#222828',
  'surface-tint': '#6fd0ca',
  'primary': '#e4e6e5',
  'primary-container': '#0a3d3b',
  'primary-fixed': '#163f3d',
  'primary-fixed-dim': '#5fa8a5',
  'secondary': '#6fd0ca',
  'secondary-container': '#0b4f4d',
  'secondary-fixed': '#163f3d',
  'secondary-fixed-dim': '#5fa8a5',
  'tertiary': '#8fd19e',
  'tertiary-container': '#0b4f4d',
  'tertiary-fixed': '#1d3a27',
  'tertiary-fixed-dim': '#5fa8a5',
  'highlight': '#f2a516',
  'highlight-container': '#3d2c0a',
  'on-background': '#e4e6e5',
  'on-surface': '#e4e6e5',
  'on-surface-variant': '#b9c3c1',
  'on-primary': '#ffffff',
  'on-primary-container': '#d5ecea',
  'on-primary-fixed': '#d5ecea',
  'on-primary-fixed-variant': '#9fd3cf',
  'on-secondary': '#ffffff',
  'on-secondary-container': '#ffffff',
  'on-secondary-fixed': '#d5ecea',
  'on-secondary-fixed-variant': '#9fd3cf',
  'on-tertiary': '#ffffff',
  'on-tertiary-container': '#6fd0ca',
  'on-tertiary-fixed': '#a3dcb0',
  'on-tertiary-fixed-variant': '#a3dcb0',
  'on-highlight': '#16181d',
  'on-highlight-container': '#f5d08a',
  'outline': '#8e9695',
  'outline-variant': '#3f4848',
  'error': '#ff8a80',
  'error-container': '#5c1a1f',
  'on-error': '#ffffff',
  'on-error-container': '#ffdad6',
  'inverse-surface': '#080b0b',
  'inverse-on-surface': '#e1e9e8',
  'inverse-primary': '#5fa8a5',
  'category-communication': '#1b2a45',
  'on-category-communication': '#a9c7ff',
  'category-erp': '#163f3d',
  'on-category-erp': '#9fd3cf',
  'category-accounting': '#3d2c0a',
  'on-category-accounting': '#f5c56b',
  'category-reporting': '#2c2448',
  'on-category-reporting': '#c9b8f5',
  'category-integration': '#2b3232',
  'on-category-integration': '#c4cfcd',
  'category-default': '#2e3133',
  'on-category-default': '#c8ccd0',
  'secondary-fill': '#0e6e6b',
  'link-hover': '#9fd3cf'
};

const channels = (hex) => [1, 3, 5].map((offset) => parseInt(hex.slice(offset, offset + 2), 16)).join(' ');

const variables = (palette) =>
  Object.fromEntries(Object.entries(palette).map(([name, hex]) => [`--c-${name}`, channels(hex)]));

const tokens = Object.fromEntries(
  Object.keys(lightColors).map((name) => [name, `rgb(var(--c-${name}) / <alpha-value>)`])
);

/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './Pages/**/*.{razor,cshtml}',
    './Shared/**/*.razor',
    './App.razor',
    './_Imports.razor'
  ],
  darkMode: 'class',
  // The ":root.dark" variables are a base rule; Tailwind drops base rules whose class never appears in the content.
  safelist: ['dark'],
  theme: {
    extend: {
      colors: tokens,
      // "secondary" is both link text (light petrol in dark mode) and the fill behind white text (stays dark petrol),
      // so background utilities read their own token.
      backgroundColor: {
        'secondary': tokens['secondary-fill']
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
  plugins: [
    plugin(({ addBase }) => {
      addBase({
        ':root': variables(lightColors),
        ':root.dark': { ...variables(darkColors), 'color-scheme': 'dark' }
      });
    })
  ]
};
