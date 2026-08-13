# Security Policy

This security policy outlines how vulnerabilities are reported, investigated, and disclosed for the Transliteration API project. We maintain security fixes for the latest stable release and encourage responsible disclosure of any discovered vulnerabilities.

## 📑 Table of Contents

- [Supported Versions](#-supported-versions)
- [Reporting a Vulnerability](#-reporting-a-vulnerability)
- [Scope](#-scope)
- [Disclosure Policy](#-disclosure-policy)
- [Safe Harbour](#-safe-harbour)
- [Recognition](#-recognition)

## 🛡️ Supported Versions

Use this table to indicate which project versions currently receive security maintenance.

| Version | Distribution Channel | Supported |
|---------|----------------------|-----------|
| Latest version | GitHub Releases | ✅ |
| Preceding versions | Any distribution channel | ❌ |

## 🚨 Reporting a Vulnerability

Please do not disclose suspected vulnerabilities publicly before maintainers have had an opportunity to validate and remediate them.

To report a vulnerability:
- [GitHub Security Advisories](https://github.com/hmlendea/transliteration-api/security/advisories)
- Contact the maintainers directly

## 📌 Scope

The subsequent report categories are in scope for this repository:
- API endpoint vulnerabilities (authentication, authorisation, input validation)
- Dependency vulnerabilities (third-party library security issues)
- Cryptographic or HMAC signing implementation flaws
- Cache security and data exposure issues
- Configuration or environment mismanagement leading to security exposure
- HTTP header injection or response manipulation

The subsequent categories are out of scope unless explicitly stated to the contrary:
- Vulnerabilities in third-party services or external transliteration providers
- User account security (this is an API without user accounts)
- Social engineering or phishing attacks
- Denial-of-service attacks without proof of concept
- Documentation or informational disclosure

## 📢 Disclosure Policy

This project follows coordinated disclosure:
1. Vulnerabilities are investigated privately.
2. A remediation plan is prepared and validated.
3. Public disclosure is published after a fix, mitigation, or agreed risk decision is available.
4. Credit is attributed in accordance with reporter preference and project policy.

## 🧾 Safe Harbour

If your research is conducted in good faith, confined to authorised scope, and disclosed responsibly, the maintainers will not pursue action for policy-compliant activity.

## 🙏 Recognition

We appreciate responsible disclosure. Reporters who desire public attribution may be acknowledged in release notes, advisories, or a dedicated acknowledgements section.
