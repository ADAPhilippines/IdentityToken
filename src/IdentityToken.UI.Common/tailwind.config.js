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
            'idt-indigo': '#4C4DE7',
            'idt-purple': '#7A60CA',
            'idt-gray': '#808088',
            'idt-gray-2': '#E8EAEF',
            'idt-gray-light': '#CFD1DC',
            'idt-gray-lightest': '#CFD1DC'
        },
        textColor: {
            ...colors,
            'primary': '#1E2835',
            'secondary': '#606066',
            'idt-gray': '#808088',
            'idt-indigo': '#4C4DE7',
            'idt-purple': '#7A60CA'
        },
        placeholderColor: {
            ...colors,
            'focused': '#1E2835',
            'unfocused': '#808088'
        },
        extend: {},
    },
    variants: {},
    plugins: [],
}
