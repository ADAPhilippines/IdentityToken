const colors = require('tailwindcss/colors')

module.exports = {
  mode: "jit",
  purge: {
    enabled: true,
    content: [
      './**/*.html',
      './**/*.razor'
    ],
  },
  darkMode: 'class', // or 'media' or 'class'
  theme: {
    container: {
      center: true,
    },
    fontFamily: {
      'sans': ['Poppins', 'sans-serif']
    },
    colors: {
      ...colors,
      'idt-indigo': '#4C4DE7'
    },
    textColor: {
      ...colors,
      'primary': '#1E2835',
      'secondary': '#606066',
      'idt-indigo': '#4C4DE7'
    },
    extend: {},
  },
  variants: {
    extend: {},
  },
  plugins: [],
}
