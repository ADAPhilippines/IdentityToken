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
    textColor: {
      'primary': '#1E2835',
      'secondary': '#606066'
    },
    extend: {},
  },
  variants: {
    extend: {},
  },
  plugins: [],
}
