find . -type f -name '*.tsx' ! -path './node_modules/*' ! -path './.next/*' -exec cat {} + > chatgpt.txt
