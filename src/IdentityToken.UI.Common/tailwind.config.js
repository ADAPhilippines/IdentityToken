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
            'transparent': 'transparent',
            'idt-indigo': '#4C4DE7',
            'idt-purple': '#7A60CA',
            'idt-gray': '#808088',
            'idt-gray-2': '#E8EAEF',
            'idt-gray-light': '#CFD1DC',
            'idt-gray-lightest': '#CFD1DC',
            'idt-green-darkest': '#1E2835',
            'idt-success': '#198754',
            'idt-danger': '#DC3545'
        },
        textColor: {
            ...colors,
            'primary': '#1E2835',
            'secondary': '#606066',
            'idt-gray': '#808088',
            'idt-gray-light': '#676C71',
            'idt-gray-light-2': '#CFD1DC',
            'idt-indigo': '#4C4DE7',
            'idt-purple': '#7A60CA'
        },
        placeholderColor: {
            ...colors,
            'focused': '#1E2835',
            'unfocused': '#808088'
        },
        extend: {
            lineHeight: {
                'extra-loose': '2.5',
                '12': '3rem',
            }
        },
    },
    variants: {},
    plugins: [],
}
