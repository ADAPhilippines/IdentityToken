const path = require('path');

module.exports = {
    entry: './wwwroot/scripts/app/app.ts',
    module: {
        rules: [
            {
                test: /\.tsx?$/,
                use: 'ts-loader',
                exclude: /node_modules/,
            }
        ],
    },
    resolve: {
        extensions: ['.tsx', '.ts', '.js'],
    },
    output: {
        filename: 'app.js',
        path: path.resolve(__dirname, './wwwroot/dist'),
    },
    experiments: {
        asyncWebAssembly: true,
    }
};